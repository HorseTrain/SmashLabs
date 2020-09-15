using SmashExporter.CrossExporter;
using SmashLabs.IO;
using SmashLabs.IO.Parsables.Material;
using SmashLabs.IO.Parsables.Material.Enums;
using SmashLabs.IO.Parsables.Mesh;
using SmashLabs.IO.Parsables.Model;
using SmashLabs.IO.Parsables.Skeleton;
using SmashLabs.Structs;
using SmashLabs.Tools.Accessors;
using System.Drawing;
using System.Collections.Generic;
using System.IO;

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

    public struct TexturePointer
    {
        public int NameOffset;
        public int TextureOffset;
        public int TextureSize;
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
            Out.Buffers.Add(ParseTextureCollection(path));

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

                    bool Rigged = false;

                    ExtendedSmashVertex[] verttemp = ExtendedSmashVertex.ReadFullVerticies(vertexAccesor, rigAccessor, skeleton, Object,out Rigged);

                    if (!Rigged)
                    {
                        for (int v = 0; v < verttemp.Length; v++)
                        {
                            verttemp[v].VertexData.VertexPosition = TransformPoint(skeleton.BoneEntries[verttemp[v].RigData.VertexWeightIndex.X].WorldTransform,verttemp[v].VertexData.VertexPosition);
                            verttemp[v].VertexData.VertexNormal = TransformNormal(skeleton.BoneEntries[verttemp[v].RigData.VertexWeightIndex.X].WorldTransform, verttemp[v].VertexData.VertexNormal);
                        }
                    }

                    CrossFile.AddArrayToObject(verttemp,ref MeshBuffer);
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
            List<byte> InverseMatrixBuffer = temp.AddBuffer();

            CrossFile.AddObjectToBuffer((short)file.BoneEntries.Length,ref PointerBuffer);

            foreach (BoneEntry entry in file.BoneEntries)
            {
                BoneEntryPointer Out = new BoneEntryPointer();

                Out.Index = entry.Index;
                Out.ParentIndex = entry.ParentIndex;

                Out.NameOffset = StringBuffer.Count;
                CrossFile.AddStringToBuffer(entry.Name,ref StringBuffer);

                CrossFile.AddObjectToBuffer(entry.LocalTransform, ref MatrixBuffer);
                CrossFile.AddObjectToBuffer(entry.InverseWorldTransform, ref InverseMatrixBuffer);

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

            EntryBuffer.Add((byte)file.Entries.Length);

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
                            CrossFile.AddObjectToBuffer((bool)attr.DataObject, ref MaterialData);
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

        static List<byte> ParseTextureCollection(string path)
        {
            CrossFile Out = new CrossFile();

            Out.Magic = "TEXTPAK";

            List<byte> PointerBuffer = Out.AddBuffer();
            List<byte> StringBuffer = Out.AddBuffer();
            List<byte> TextureBuffer = Out.AddBuffer();

            string[] files = Directory.GetFiles(path);

            int count = 0;

            foreach (string file in files)
            {
                if (file.EndsWith(".nutexb"))
                {
                    count++;

                    TexturePointer pointer = new TexturePointer();

                    pointer.NameOffset = StringBuffer.Count;
                    CrossFile.AddStringToBuffer(Path.GetFileName(file).Split('.')[0],ref StringBuffer);

                    pointer.TextureOffset = TextureBuffer.Count;

                    Bitmap temp = NUTEXTB.TextureLoader.LoadNUTEXTB(file);

                    temp.Save("temp.png");
                    byte[] dat = File.ReadAllBytes("temp.png");
                    pointer.TextureSize = dat.Length;
                    CrossFile.AddArrayToObject(dat,ref TextureBuffer);
                  
                    CrossFile.AddObjectToBuffer(pointer,ref PointerBuffer);
                }
            }

            PointerBuffer.Insert(0,(byte)count);

            Out.BuildFile();

            return Out.FinalBuffer;
        }

        public static Vector3 TransformPoint(Matrix4 mat,Vector3 vec)
        {
            OpenTK.Vector3 temp = OpenTK.Vector3.TransformPosition(new OpenTK.Vector3(vec.X, vec.Y, vec.Z), ConvertMatrix(mat));

            return new Vector3(temp.X,temp.Y,temp.Z);
        }

        public static Vector3 TransformNormal(Matrix4 mat, Vector3 vec)
        {
            OpenTK.Vector3 temp = OpenTK.Vector3.TransformNormal(new OpenTK.Vector3(vec.X, vec.Y, vec.Z), ConvertMatrix(mat));

            return new Vector3(temp.X, temp.Y, temp.Z);
        }


        public static unsafe OpenTK.Matrix4 ConvertMatrix(Matrix4 mat)
        {
            OpenTK.Matrix4* temp = (OpenTK.Matrix4*)&mat;

            return *temp;
        }
    }
}
