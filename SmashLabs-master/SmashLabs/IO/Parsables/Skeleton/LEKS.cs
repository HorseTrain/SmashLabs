using SmashLabs.Structs;
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

        public override void LoadData()
        {
            long EntryDataLocation = reader.ReadOffset();

            BoneEntries = new BoneEntry[reader.ReadLong()];

            long MatrixDataLocation = reader.ReadOffset();

            reader.Seek(EntryDataLocation);

            for (int i = 0; i < BoneEntries.Length; i++)
            {
                BoneEntries[i] = new BoneEntry()
                {
                    Name = reader.ReadStringOffset(),
                    Index = reader.ReadInt(),
                    Type = reader.ReadInt()
                };
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
    }
}
