using SmashLabs.IO.Parsables.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Exporter
{
    public struct SkeletonPointer
    {
        public int NameOffset;
        public int ID;
        public int PatentId;
    }

    public struct Transform
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;

        public static Transform FromMatrix(Matrix4 matrix)
        {
            Transform Out = new Transform();

            Out.LocalPosition = matrix.ExtractTranslation();
            Out.LocalRotation = matrix.ExtractRotation();
            Out.LocalScale = matrix.ExtractScale();

            return Out;
        }
    }

    public static partial class Exporters
    {    
        public static void AddSkeletonToCrossFile(CrossFile file,LEKS skeleton)
        {
            CrossFile temp = new CrossFile();
            List<byte> PointerBuffer = temp.AddBuffer();
            List<byte> TransformBuffer = temp.AddBuffer();
            List<byte> InverseMatrixBuffer = temp.AddBuffer();
            List<byte> StringBuffer = temp.AddBuffer();

            temp.Magic = "SKEL";

            CrossFile.AddObjectToArray((short)skeleton.BoneEntries.Length,PointerBuffer);

            foreach (BoneEntry entry in skeleton.BoneEntries)
            {
                SkeletonPointer pointer = new SkeletonPointer();

                pointer.NameOffset = StringBuffer.Count;
                CrossFile.AddStringToBuffer(entry.Name,StringBuffer);

                pointer.ID = entry.Index;
                pointer.PatentId = entry.ParentIndex;
                
                CrossFile.AddObjectToArray(Transform.FromMatrix(FromSBMatrix(entry.LocalTransform)), TransformBuffer);
                CrossFile.AddObjectToArray(entry.InverseWorldTransform, InverseMatrixBuffer);

                CrossFile.AddObjectToArray(pointer,PointerBuffer);
            }

            temp.BuildBuffer();

            file.Buffers.Add(temp.FinalBuffer);
        }

        public static unsafe Matrix4 FromSBMatrix(SmashLabs.Structs.Matrix4 matrix)
        {
            return *(Matrix4*)&matrix;
        }
    }
}
