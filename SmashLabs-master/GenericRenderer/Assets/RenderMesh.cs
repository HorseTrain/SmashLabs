using GenericRenderer.Game;
using GenericRenderer.Structs;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericRenderer.Assets
{
    public unsafe class RenderMesh
    {
        public BeginMode DrawMode { get; set; } = BeginMode.Triangles;
        public RenderVertex* VertexBuffer { get; set; } //Make sure to always pin datta source.
        public ushort* IndexBuffer { get; set; } //Make sure to always pin datta source.
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }
        public int VAO { get; private set; }
        public int IBO { get; private set; }
        public int VBO { get; private set; }

        public void Upload()
        {
            VAO = GL.GenVertexArray();
            IBO = GL.GenBuffer();
            VBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer,IndexCount * sizeof(ushort),(IntPtr)VertexBuffer,BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            GL.BufferData(BufferTarget.ArrayBuffer,VertexCount * sizeof(RenderVertex),(IntPtr)VertexBuffer,BufferUsageHint.StaticDraw);
        }
        public void Bind()
        {
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer,IBO);
        }
        public void GenericDraw()
        {

        }
        ~RenderMesh()
        {
            if (Window.MainWindow.Active)
            {
                GL.DeleteVertexArray(VAO);
                GL.DeleteBuffer(IBO);
                GL.DeleteBuffer(VBO);
            }
        }
    }
}
