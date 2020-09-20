using SmashLabs.Structs;
using SmashLabs.Tools.Exporters;
using SmashLabs.Tools.Exporters.Structs.Formats.LEKS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.IO.Parsables.Skeleton
{
    //Use updated model.
    public class LEKS : IParsable
    {
        public BoneEntry[] BoneEntries { get; set; }
        public Dictionary<string,int> BoneDic { get; set; }

        public override void LoadData()
        {
            long EntryDataLocation = reader.ReadOffset();

            BoneEntries = new BoneEntry[reader.ReadLong()];

            long MatrixDataLocation = reader.ReadOffset();

            reader.Seek(EntryDataLocation);

            BoneDic = new Dictionary<string, int>();

            for (int i = 0; i < BoneEntries.Length; i++)
            {
                BoneEntries[i] = new BoneEntry()
                {
                    Name = reader.ReadStringOffset(),
                    Index = reader.ReadShort(),
                    ParentIndex = reader.ReadShort(),
                    Type = reader.ReadInt()
                };

                BoneDic.Add(BoneEntries[i].Name,BoneEntries[i].Index);
            }

            reader.Seek(MatrixDataLocation);

            Matrix4[] WorldTransforms = reader.ReadObject<Matrix4>(BoneEntries.Length);
            Matrix4[] InverseWorldTransforms = reader.ReadObject<Matrix4>(BoneEntries.Length);
            Matrix4[] Transforms = reader.ReadObject<Matrix4>(BoneEntries.Length);
            Matrix4[] InverseTransforms = reader.ReadObject<Matrix4>(BoneEntries.Length);

            for (int i = 0; i < BoneEntries.Length; i++)
            {
                BoneEntries[i].WorldTransform = WorldTransforms[i];
                BoneEntries[i].InverseWorldTransform = InverseWorldTransforms[i];
                BoneEntries[i].LocalTransform = Transforms[i];
                BoneEntries[i].InverseLocalTransform = InverseTransforms[i];
            }
        }

        public override unsafe byte[] GetData()
        {
            ExportableBuffer Exporter = new ExportableBuffer();

            ByteBuffer HeaderData = Exporter.AddBuffer();
            ByteBuffer PointerData = Exporter.AddBuffer();

            ByteBuffer StringBuffer = Exporter.AddBuffer();

            {
                int pointersize = BoneEntries.Length * 16;

                foreach (BoneEntry entry in BoneEntries)
                {
                    PointerData.AddObjectToArray((long)pointersize + StringBuffer.Count);
                    StringBuffer.AddStringToBuffer(entry.Name);

                    PointerData.AddObjectToArray(entry.Index);
                    PointerData.AddObjectToArray(entry.ParentIndex);
                    PointerData.AddObjectToArray(entry.Type);

                    pointersize -= 16;
                }
            }

            {
                ByteBuffer WorldTransform = Exporter.AddBuffer();
                ByteBuffer InverseWorldTransform = Exporter.AddBuffer();
                ByteBuffer TransformBuffer = Exporter.AddBuffer();
                ByteBuffer InverseTransformBuffer = Exporter.AddBuffer();

                foreach (BoneEntry entry in BoneEntries)
                {
                    WorldTransform.AddObjectToArray(entry.WorldTransform);
                    InverseWorldTransform.AddObjectToArray(entry.InverseWorldTransform);
                    TransformBuffer.AddObjectToArray(entry.LocalTransform);
                    InverseTransformBuffer.AddObjectToArray(entry.InverseLocalTransform);
                }
            }

            {
                HeaderData.AddObjectToArray(Header);
                HeaderData.AddObjectToArray(Magic);

                HeaderData.AddObjectToArray(80L);

                HeaderData.AddObjectToArray((long)BoneEntries.Length);
                HeaderData.AddObjectToArray((long)PointerData.Count + StringBuffer.Count + (104 - HeaderData.Count));

                HeaderData.AddObjectToArray((long)BoneEntries.Length);
                HeaderData.AddObjectToArray((long)PointerData.Count + StringBuffer.Count + (104 - HeaderData.Count) + (1 * (sizeof(Matrix4) * BoneEntries.Length)));

                HeaderData.AddObjectToArray((long)BoneEntries.Length);
                HeaderData.AddObjectToArray((long)PointerData.Count + StringBuffer.Count + (104 - HeaderData.Count) + (2 * (sizeof(Matrix4) * BoneEntries.Length)));

                HeaderData.AddObjectToArray((long)BoneEntries.Length);
                HeaderData.AddObjectToArray((long)PointerData.Count + StringBuffer.Count + (104 - HeaderData.Count) + (3 * (sizeof(Matrix4) * BoneEntries.Length)));

                HeaderData.AddObjectToArray((long)BoneEntries.Length);
            }

            return Exporter.BuildFinalBuffer().ToArray();
        }
    }
}
