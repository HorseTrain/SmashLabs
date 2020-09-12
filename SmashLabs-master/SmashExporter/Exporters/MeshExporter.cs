using SmashExporter.CrossExporter;
using SmashLabs.IO;
using SmashLabs.IO.Parsables.Material;
using SmashLabs.IO.Parsables.Material.Enums;
using SmashLabs.IO.Parsables.Mesh;
using SmashLabs.IO.Parsables.Model;
using SmashLabs.IO.Parsables.Skeleton;
using SmashLabs.Structs;
using SmashLabs.Tools.Accessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace SmashExporter
{
    public struct MeshObjectPointer
    {
        public int NameOffset;
        public int MaterialNameOffset;
        public int VertexCount;
        public int IndexCount;
        public int IndexBufferOffset;
        public int VertexBufferOffset;
    }

    public struct BoneEntryPointer
    {
        public short Index;
        public short ParentIndex;
        public int NameOffset;
    }

    public struct MaterialEntryPointer
    {
        public int LabelOffset;
        public int ShaderLabelOffset;
        public byte MaterialAttributeCount;
        public int MaterialAttributeOffset;
    }
    public struct MaterialAttributePointer
    {
        public ParamID ID;
        public ParamDataType Type;
        public int DataOffset;
    }

    public partial class Exporters
    {
        public unsafe static CrossFile GetModelFile(string path)
        {
            CrossFile Out = new CrossFile();

            Out.Magic = "MODEL";

            HSEM mesh = (HSEM)IParsable.FromFile(path + "\\model.numshb");
            LEKS skeleton = (LEKS)IParsable.FromFile(path + "\\model.nusktb");
            LDOM model = (LDOM)IParsable.FromFile(path + "\\model.numdlb");
            LTAM material = (LTAM)IParsable.FromFile(path + "\\model.numatb");

            Out.Buffers.Add(ParseMeshCollection(mesh,skeleton,model));
            Out.Buffers.Add(ParseSkeletonFile(skeleton));
            Out.Buffers.Add(ParseMaterialFile(material));

            return Out;
        }

        static List<byte> ParseMeshCollection(HSEM file,LEKS skeleton,LDOM model)
        {
            CrossFile temp = new CrossFile();

            temp.Magic = "MESH";

            List<byte> PointerBuffer = temp.AddBuffer();
            List<byte> StringBuffer = temp.AddBuffer();
            List<byte> MeshBuffer = temp.AddBuffer();

            VertexAccesor vertexAccesor = new VertexAccesor(file);
            RigAccessor rigAccessor = new RigAccessor(file);

            CrossFile.AddObjectToBuffer((byte)file.Objects.Length,ref PointerBuffer);

            for (int i = 0; i < file.Objects.Length; i++)
            {
                MeshObject Object = file.Objects[i];
                ModelEntry Entry = model.Entries[i];

                MeshObjectPointer Out = new MeshObjectPointer();

                Out.IndexCount = Object.MeshData.IndexCount;
                Out.VertexCount = Object.MeshData.VertexCount;

                {
                    Out.NameOffset = StringBuffer.Count;
                    CrossFile.AddStringToBuffer(Entry.Name, ref StringBuffer);

                    Out.MaterialNameOffset = StringBuffer.Count;
                    CrossFile.AddStringToBuffer(Entry.MaterialName, ref StringBuffer);
                }

                {
                    Out.IndexBufferOffset = MeshBuffer.Count;
                    CrossFile.AddArrayToObject(vertexAccesor.ReadIndiciesShort(Object),ref MeshBuffer);

                    Out.VertexBufferOffset = MeshBuffer.Count;
                    CrossFile.AddArrayToObject(ExtendedSmashVertex.ReadFullVerticies(vertexAccesor,rigAccessor,skeleton,Object),ref MeshBuffer);
                }

                CrossFile.AddObjectToBuffer(Out,ref PointerBuffer);
            }

            temp.BuildFile();

            return temp.FinalBuffer;
        }

        static List<byte> ParseSkeletonFile(LEKS file)
        {
            CrossFile temp = new CrossFile();

            temp.Magic = "SKEL";

            List<byte> PointerBuffer = temp.AddBuffer();
            List<byte> StringBuffer = temp.AddBuffer();
            List<byte> MatrixBuffer = temp.AddBuffer();

            CrossFile.AddObjectToBuffer((short)file.BoneEntries.Length,ref PointerBuffer);

            foreach (BoneEntry entry in file.BoneEntries)
            {
                BoneEntryPointer Out = new BoneEntryPointer();

                Out.Index = entry.Index;
                Out.ParentIndex = entry.ParentIndex;

                Out.NameOffset = StringBuffer.Count;
                CrossFile.AddStringToBuffer(entry.Name,ref StringBuffer);

                CrossFile.AddObjectToBuffer(entry.LocalTransform, ref MatrixBuffer);

                CrossFile.AddObjectToBuffer(Out,ref PointerBuffer);
            }

            temp.BuildFile();

            return temp.FinalBuffer;
        }

        static List<byte> ParseMaterialFile(LTAM file)
        {
            CrossFile temp = new CrossFile();

            temp.Magic = "MATL";

            List<byte> EntryBuffer = temp.AddBuffer();
            List<byte> StringBuffer = temp.AddBuffer();

            List<byte> MaterialAttributeData = temp.AddBuffer();
            List<byte> MaterialData = temp.AddBuffer();

            EntryBuffer.Add((byte)EntryBuffer.Count);

            foreach (MaterialEntry entry in file.Entries)
            {
                MaterialEntryPointer pointer = new MaterialEntryPointer();

                pointer.LabelOffset = StringBuffer.Count;
                CrossFile.AddStringToBuffer(entry.Label, ref StringBuffer);

                pointer.ShaderLabelOffset = StringBuffer.Count;
                CrossFile.AddStringToBuffer(entry.ShaderLabel, ref StringBuffer);

                pointer.MaterialAttributeCount = (byte)entry.Attributes.Length;
                pointer.MaterialAttributeOffset = MaterialAttributeData.Count;

                foreach (MaterialAttribute attr in entry.Attributes)
                {
                    MaterialAttributePointer atpointer = new MaterialAttributePointer();

                    atpointer.ID = attr.paramID;
                    atpointer.Type = attr.DataType;

                    atpointer.DataOffset = MaterialData.Count;

                    switch (attr.DataType)
                    {
                        case ParamDataType.Float:
                            CrossFile.AddObjectToBuffer((float)attr.DataObject,ref MaterialData);
                            break;
                        case ParamDataType.Boolean:
                            CrossFile.AddObjectToBuffer((bool)attr.DataObject, ref MaterialData); ;
                            break;
                        case ParamDataType.Vector4:
                            CrossFile.AddObjectToBuffer((Vector4)attr.DataObject, ref MaterialData);
                            break;
                        case ParamDataType.String:
                            CrossFile.AddStringToBuffer((string)attr.DataObject,ref MaterialData);
                            break;
                        case ParamDataType.Sampler:
                            CrossFile.AddObjectToBuffer((Sampler)attr.DataObject, ref MaterialData);
                            break;
                        case ParamDataType.UvTransform:
                            CrossFile.AddObjectToBuffer((UVTransform)attr.DataObject, ref MaterialData);
                            break;
                        case ParamDataType.BlendState:
                            CrossFile.AddObjectToBuffer((BlendState)attr.DataObject, ref MaterialData);
                            break;
                        case ParamDataType.RasterizerState:
                            CrossFile.AddObjectToBuffer((RasterizerState)attr.DataObject, ref MaterialData);
                            break;
                    }

                    CrossFile.AddObjectToBuffer(atpointer,ref MaterialAttributeData);
                }

                CrossFile.AddObjectToBuffer(pointer,ref EntryBuffer);
            }

            temp.BuildFile();

            return temp.FinalBuffer;
        }
    }
}
