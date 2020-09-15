using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericRenderer.Assets
{
    public class BoneNode
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public int ParentID { get; set; }
        BoneNode parent { get; set; }
        public BoneNode Parent 
        {            
            get => parent; 

            set 
            { 
                if (parent != null)
                {
                    parent.Children.Remove(this);
                }

                parent = value;

                parent.Children.Add(this);
            } 
        }
        List<BoneNode> Children { get; set; } = new List<BoneNode>();
        public BoneNode GetBone(int index)
        {
            return Children[index];
        }
        public int ChildrenCount => Children.Count;

        public Vector3 LocalPosition { get; set; }
        public Quaternion LocalRotation  { get; set; }
        public Vector3 LocalScale { get; set; }
        public Matrix4 LocalTransform
        {
            get
            {
                return TRS(LocalPosition, LocalRotation, LocalScale);
            }

            set
            {
                LocalPosition = value.ExtractTranslation();
                LocalRotation = value.ExtractRotation();
                LocalScale = value.ExtractScale();
            }
        }

        public static Matrix4 TRS(Vector3 translation,Quaternion rotation,Vector3 scale)
        {
            return

                Matrix4.CreateScale(scale) * Matrix4.CreateFromQuaternion(rotation) * Matrix4.CreateTranslation(translation);
        }

        public Matrix4 WorldTransform //Could improve on 
        {
            get
            {
                if (parent == null)
                {
                    return LocalTransform;
                }
                else
                {
                    return LocalTransform * parent.WorldTransform;
                }
            }
        }
    }

    public class RenderSkeleton
    {
        public UniformBuffer<Matrix4> SkeletonBuffer { get; set; }
        public BoneNode[] Nodes { get; set; }
        public Matrix4[] InverseWorldTransforms { get; set; }
        public Dictionary<string, BoneNode> NodeDictonary { get; set; }

        public void BuildDic()
        {
            NodeDictonary = new Dictionary<string, BoneNode>();

            foreach (BoneNode node in Nodes)
            {
                NodeDictonary.Add(node.Name,node);
            }
        }

        public BoneNode GetNode(string name)
        {
            if (NodeDictonary.ContainsKey(name))
                return NodeDictonary[name];
            else
                return null;
        }

        public void RecalculateInverseWorldTransforms() //Slow!!
        {
            InverseWorldTransforms = new Matrix4[Nodes.Length];

            for (int i = 0; i < Nodes.Length; i++)
            {
                try
                {
                    InverseWorldTransforms[i] = Matrix4.Invert(Nodes[i].WorldTransform);
                }
                catch
                {
                    InverseWorldTransforms[i] = Matrix4.Identity;
                }

            }
        }

        public RenderSkeleton()
        {
            SkeletonBuffer = new UniformBuffer<Matrix4>(300, "SkeletonBuffer");
        }

        public void BufferData()
        {
            for (int i = 0; i < Nodes.Length; i++)
            {
                SkeletonBuffer[i] = InverseWorldTransforms[i] * Nodes[i].WorldTransform;
            }

            SkeletonBuffer.BufferData();
        }

        public void BindSkeleton(RenderMaterial material)
        {
            SkeletonBuffer.BindBufferToShader(material,1);
        }
    }
}
