using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace GenericRenderer.Assets
{
    public unsafe class UniformBuffer<T> where T : unmanaged
    {
        public int Handler { get; private set; }
        public string Name { get; set; } = "";
        public T[] Buffer { get; set; }
        public bool isSetUp { get; private set; } = false;

        public UniformBuffer(int size,string Name)
        {
            Buffer = new T[size];
            this.Name = Name;
        }

        void SetUp()
        {
            if (!isSetUp)
            {
                Handler = GL.GenBuffer();

                isSetUp = true;
            }
        }

        public void BufferData()
        {
            SetUp();

            GL.BindBuffer(BufferTarget.UniformBuffer,Handler);
            GL.BufferData(BufferTarget.UniformBuffer,sizeof(T) * Buffer.Length,Buffer,BufferUsageHint.StaticDraw);
        }

        public void BindBufferToShader(RenderMaterial material,int ShaderIndex = 0)
        {
            SetUp();

            int num = material.CurrentShader.GetSahderUniformBlock(Name);
            if (num == -1)
                num = GL.GetUniformBlockIndex(material.CurrentShader.Handler, Name);
            if (num == -1)
                return;
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, num, Handler);
            material.CurrentShader.UniformInt(Name, num);
            GL.UniformBlockBinding(material.CurrentShader.Handler, num, ShaderIndex);
        }

        public ref T this[int index] => ref Buffer[index];
    }
}
