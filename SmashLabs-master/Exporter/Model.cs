using System;
using System.Collections.Generic;
using System.Net;
using SmashLabs.IO;
using SmashLabs.IO.Parsables.Mesh;
using SmashLabs.IO.Parsables.Model;
using SmashLabs.IO.Parsables.Skeleton;
using SmashLabs.Structs;
using SmashLabs.Tools.Accessors;

namespace Exporter
{
    public struct MeshObjectHeader
    {
        public int NameOffset;
        public int MaterialNameOffset;
        public int VertexCount;
        public int IndexCount;
        public int IndexOffset;
        public int VertexOffset;
        public int RigDataOffset;
    }

    public static partial class Exporters
    {
        public static int[] ToIntArray(uint[] Data)
        {
            int[] Out = new int[Data.Length];

            for (int i = 0; i < Out.Length; i++)
            {
                Out[i] = (int)Data[i];
            }

            return Out;
        }

        public static void AddModelToCrossFile(CrossFile file,string path)
        {
            CrossFile Temp = new CrossFile();

            List<byte> meshpointers = Temp.AddBuffer();
            List<byte> VertexBuffer = Temp.AddBuffer();
            List<byte> RigBuffer = Temp.AddBuffer();
            List<byte> stringbuffers = Temp.AddBuffer();

            LEKS skeleton = (LEKS)IParsable.FromFile(path + "\\model.nusktb");

            Temp.Magic = "MODEL";

            LDOM modelfile = (LDOM)IParsable.FromFile(path + "\\model.numdlb");
            HSEM meshfile = (HSEM)IParsable.FromFile(path + "\\model.numshb");

            VertexAccesor accesor = new VertexAccesor(meshfile);
            RigAccessor raccessor = new RigAccessor(meshfile);

            CrossFile.AddObjectToArray((short)modelfile.Entries.Length,meshpointers);

            for (int i = 0; i < modelfile.Entries.Length; i++)
            {
                ModelEntry entry = modelfile.Entries[i];
                MeshObject Object = meshfile.Objects[i];

                MeshObjectHeader header = new MeshObjectHeader();

                header.IndexCount = Object.MeshData.IndexCount;
                header.VertexCount = Object.MeshData.VertexCount;

                header.NameOffset = stringbuffers.Count;
                CrossFile.AddStringToBuffer(entry.Name,stringbuffers);

                header.MaterialNameOffset = stringbuffers.Count;
                CrossFile.AddStringToBuffer(entry.MaterialName, stringbuffers);

                header.IndexOffset = VertexBuffer.Count;
                CrossFile.AddObjectArrayToArray(ToIntArray(accesor.ReadIndicies(Object)), VertexBuffer);

                bool rigged = false;

                header.RigDataOffset = RigBuffer.Count;
                CrossFile.AddObjectArrayToArray(raccessor.ReadVertexWeightData(Object,skeleton.BoneDic,out rigged),RigBuffer);

                header.VertexOffset = VertexBuffer.Count;

                SmashVertex[] VertexData = accesor.ReadVertexData(Object);

                if (!rigged)
                {
                    OpenTK.Matrix4 Transform = FromSBMatrix(skeleton.BoneEntries[skeleton.BoneDic[Object.ParentBoneName]].WorldTransform);

                    for (int v = 0; v < VertexData.Length; v++)
                    {
                        {
                            OpenTK.Vector3 NewPoint = new OpenTK.Vector3(VertexData[v].VertexPosition.X, VertexData[v].VertexPosition.Y, VertexData[v].VertexPosition.Z);

                            NewPoint = OpenTK.Vector3.TransformPosition(NewPoint, Transform);

                            VertexData[v].VertexPosition = new Vector3(NewPoint.X, NewPoint.Y, NewPoint.Z);
                        }

                        {
                            OpenTK.Vector3 NewNormal = new OpenTK.Vector3(VertexData[v].VertexNormal.X, VertexData[v].VertexNormal.Y, VertexData[v].VertexNormal.Z);

                            NewNormal = OpenTK.Vector3.TransformNormal(NewNormal, Transform);

                            VertexData[v].VertexNormal = new Vector3(NewNormal.X, NewNormal.Y, NewNormal.Z);
                        }
                    }
                }

                CrossFile.AddObjectArrayToArray(VertexData, VertexBuffer);

                CrossFile.AddObjectToArray(header,meshpointers);
            }

            Temp.BuildBuffer();
            file.Buffers.Add(Temp.FinalBuffer);

            AddSkeletonToCrossFile(file, skeleton);

        }
    }
}
