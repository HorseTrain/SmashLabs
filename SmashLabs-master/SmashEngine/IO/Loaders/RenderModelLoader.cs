using GenericRenderer.Assets;
using GenericRenderer.Structs;
using GenericRenderer.Assets.Enums;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System;
using OpenTK;
using SmashEngine.Graphics.MaterialAttributes;

namespace SmashEngine.IO.Loaders
{
    public class RenderModelLoader
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

        public static unsafe RenderMesh ParseRenderMesh(MeshObjectPointer pointer,CrossFile file)
        {
            RenderMesh Out = new RenderMesh();

            Out.Name = file.ReadString(pointer.NameOffset + file.Pointers[1].Offset);
            Out.IndexCount = pointer.IndexCount;
            Out.VertexCount = pointer.VertexCount;
            Out.VertexBuffer = (RenderVertex*)&(file.Pointers[2].Location[pointer.VertexBufferOffset]);
            Out.IndexBuffer = (ushort*)&(file.Pointers[2].Location[pointer.IndexBufferOffset]);

            return Out;
        }

        public static unsafe RenderModel LoadModel(byte[] Data)
        {
            RenderModel Out = new RenderModel();

            Out.MeshBufferHandle = CrossFile.CreateHandle(Data);

            CrossFile MainFile = new CrossFile(CrossFile.GetArrayPointer(Data));

            Dictionary<string, RenderTexture> LoadedTextures = new Dictionary<string, RenderTexture>();

            CrossFile TextureCollectionFile = new CrossFile(MainFile.Pointers[3].Location);
            {
                TextureCollectionFile.Seek(TextureCollectionFile.Pointers[0].Offset);

                byte count = TextureCollectionFile.ReadObject<byte>();

                for (int i = 0; i < count; i++)
                {
                    TexturePointer pointer = TextureCollectionFile.ReadObject<TexturePointer>();

                    string name = TextureCollectionFile.ReadString(pointer.NameOffset + TextureCollectionFile.Pointers[1].Offset);

                    Bitmap map = RenderTexture.BitmapFromPointer(&(TextureCollectionFile.Data[pointer.TextureOffset + TextureCollectionFile.Pointers[2].Offset]),pointer.TextureSize);

                    LoadedTextures.Add(name,new RenderTexture(map));
                }
            }

            CrossFile MaterialFile = new CrossFile(MainFile.Pointers[2].Location);
            {
                MaterialFile.Seek(MaterialFile.Pointers[0].Offset);

                byte count = MaterialFile.ReadObject<byte>();

                for (int i = 0; i < count; i++)
                {
                    MaterialEntryPointer pointer = MaterialFile.ReadObject<MaterialEntryPointer>();

                    RenderMaterial OutMat = new RenderMaterial();

                    OutMat.Name = MaterialFile.ReadString(pointer.LabelOffset + MaterialFile.Pointers[1].Offset);

                    long lp = MaterialFile.BufferLocation;

                    MaterialFile.Seek(pointer.MaterialAttributeOffset + MaterialFile.Pointers[2].Offset);

                    for (int e = 0; e < pointer.MaterialAttributeCount; e++)
                    {
                        MaterialAttributePointer apointer = MaterialFile.ReadObject<MaterialAttributePointer>();

                        object TempData = null;

                        switch (apointer.Type)
                        {
                            case ParamDataType.Float:

                                TempData = MaterialFile.ReadObjectDirect<float>(apointer.DataOffset + MaterialFile.Pointers[3].Offset);

                                break;
                            case ParamDataType.Boolean:

                                TempData = MaterialFile.ReadObjectDirect<bool>(apointer.DataOffset + MaterialFile.Pointers[3].Offset);

                                break;
                            case ParamDataType.Vector4:

                                TempData = MaterialFile.ReadObjectDirect<Vector4>(apointer.DataOffset + MaterialFile.Pointers[3].Offset);

                                break;
                            case ParamDataType.String:

                                string Str = MaterialFile.ReadString(MaterialFile.Pointers[3].Offset + apointer.DataOffset);

                                if (apointer.ID.ToString().Contains("Texture"))
                                {
                                    if (LoadedTextures.ContainsKey(Str))
                                        OutMat.Textures[(ulong)apointer.ID - 92] = LoadedTextures[Str];
                                }

                                break;
                            case ParamDataType.Sampler:

                                TempData = MaterialFile.ReadObjectDirect<Sampler>(apointer.DataOffset + MaterialFile.Pointers[3].Offset);

                                break;
                            case ParamDataType.UvTransform:

                                TempData = MaterialFile.ReadObjectDirect<UVTransform>(apointer.DataOffset + MaterialFile.Pointers[3].Offset);

                                break;
                            case ParamDataType.BlendState:

                                TempData = MaterialFile.ReadObjectDirect<BlendState>(apointer.DataOffset + MaterialFile.Pointers[3].Offset);

                                break;
                            case ParamDataType.RasterizerState:

                                TempData = MaterialFile.ReadObjectDirect<RasterizerState>(apointer.DataOffset + MaterialFile.Pointers[3].Offset);

                                break;
                        }

                        if (apointer.Type != ParamDataType.String)
                        {
                            OutMat.Attributes.Add(new MaterialAttribute(){UniformName = apointer.ID.ToString(),Type = apointer.Type,Data = TempData});
                        }
                    }

                    OutMat.Shaders.Add("FighterShader",RenderShader.ShaderCollection["Fighter"]);
                    OutMat.UseShader("FighterShader");

                    MaterialFile.Seek(lp);

                    Out.Materials.Add(OutMat.Name,OutMat);
                }
            }

            CrossFile MeshCollection = new CrossFile(MainFile.Pointers[0].Location);
            {
                MeshCollection.Seek(MeshCollection.Pointers[0].Offset);

                byte MeshCollectionCount = MeshCollection.ReadObject<byte>();

                MeshObjectPointer* RenderMeshData = (MeshObjectPointer*)&MeshCollection.Data[MeshCollection.BufferLocation];

                for (int i = 0; i < MeshCollectionCount; i++)
                {
                    Out.Meshes.Add(ParseRenderMesh(RenderMeshData[i],MeshCollection));

                    Out.Meshes[Out.Meshes.Count - 1].Material = Out.Materials[MeshCollection.ReadString(RenderMeshData[i].MaterialNameOffset + MeshCollection.Pointers[1].Offset)];
                }
            }

            CrossFile Skeleton = new CrossFile(MainFile.Pointers[1].Location);
            {
                Skeleton.Seek(Skeleton.Pointers[0].Offset);

                short Count = Skeleton.ReadObject<short>();

                Out.Skeleton = new RenderSkeleton();

                Out.Skeleton.Nodes = new BoneNode[Count];

                BoneEntryPointer[] Pointers = Skeleton.ReadObject<BoneEntryPointer>(Count);

                Out.Skeleton.InverseWorldTransforms = new Matrix4[Count];

                for (int i = 0; i < Count; i++)
                {
                    BoneEntryPointer pointer = Pointers[i];

                    BoneNode Temp = new BoneNode();

                    Temp.ID = pointer.Index;
                    Temp.ParentID = pointer.ParentIndex;

                    Temp.Name = Skeleton.ReadString(pointer.NameOffset + Skeleton.Pointers[1].Offset);
                    Temp.LocalTransform = ((Matrix4*)Skeleton.Pointers[2].Location)[Temp.ID];
                    Out.Skeleton.InverseWorldTransforms[i] = ((Matrix4*)Skeleton.Pointers[3].Location)[Temp.ID];

                    Out.Skeleton.Nodes[i] = Temp;
                }

                for (int i = 0; i < Count; i++)
                {
                    if (Pointers[i].ParentIndex != -1)
                    Out.Skeleton.Nodes[i].Parent = Out.Skeleton.Nodes[Pointers[i].ParentIndex];
                }

                Out.Skeleton.BuildDic();
            }


            return Out;
        }


    }
}
