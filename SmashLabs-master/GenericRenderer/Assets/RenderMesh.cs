using GenericRenderer.Game;
using GenericRenderer.Structs;
using OpenTK.Graphics.OpenGL;
using System;
using System.Runtime.InteropServices;

namespace GenericRenderer.Assets
{
    public unsafe class RenderMesh
    {
        public string Name { get; set; } //Does not have to be filled
        public BeginMode DrawMode { get; set; } = BeginMode.Triangles;
        public RenderVertex* VertexBuffer { get; set; } //Make sure to always pin datta source.
        public ushort* IndexBuffer { get; set; } //Make sure to always pin datta source.
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }
        public int VAO { get; private set; }
        public int IBO { get; private set; }
        public int VBO { get; private set; }
        public bool Uploaded { get; private set; } = false;
        public RenderMaterial Material { get; set; }
        public void Upload()
        {
            if (!Uploaded)
            {
                VAO = GL.GenVertexArray();
                VBO = GL.GenBuffer();
                IBO = GL.GenBuffer();

                GL.BindVertexArray(VAO);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer,IBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer,IndexCount * sizeof(ushort),(IntPtr)IndexBuffer,BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
                GL.BufferData(BufferTarget.ArrayBuffer,VertexCount * sizeof(RenderVertex),(IntPtr)VertexBuffer,BufferUsageHint.StaticDraw);

                VertexAttribArray(0,3,(int)VertexAttribPointerType.Float,Marshal.OffsetOf(typeof(RenderVertex),"VertexPosition"));
                VertexAttribArray(1, 3, (int)VertexAttribPointerType.Float, Marshal.OffsetOf(typeof(RenderVertex), "VertexNormal"));

                VertexAttribArray(2, 2, (int)VertexAttribPointerType.Float, Marshal.OffsetOf(typeof(RenderVertex), "VertexMap0"));
                VertexAttribArray(3, 2, (int)VertexAttribPointerType.Float, Marshal.OffsetOf(typeof(RenderVertex), "VertexMap1"));

                VertexAttribArray(4, 4, (int)VertexAttribPointerType.Float, Marshal.OffsetOf(typeof(RenderVertex), "VertexColor"));

                VertexAttribArray(5, 4, (int)VertexAttribPointerType.Int, Marshal.OffsetOf(typeof(RenderVertex), "VertexWeightIndex"));

                VertexAttribArray(6, 4, (int)VertexAttribPointerType.Float, Marshal.OffsetOf(typeof(RenderVertex), "VertexWeight"));

                Uploaded = true;               
            }                                  
        }             
        
        public static void VertexAttribArray(int location, int size, int type,IntPtr offset)
        {
            if (type >= (int)VertexAttribType.UnsignedInt)
            {
                GL.VertexAttribPointer(location,size,(VertexAttribPointerType)type,false,sizeof(RenderVertex),offset);
            }
            else
            {
                GL.VertexAttribIPointer(location,size,(VertexAttribIPointerType)type,sizeof(RenderVertex),offset);
            }

            GL.EnableVertexAttribArray(location);
        }

        public void Bind()                     
        {                                      
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer,IBO);
        }
        public void GenericDraw()
        {
            if (Material != null)
                Material.UseMaterial();

            Upload();

            Bind();

            GL.DrawElements(BeginMode.Triangles,IndexCount,DrawElementsType.UnsignedShort,0);

            RenderMaterial.BindNullMaterial();
        }
        ~RenderMesh()
        {
            if (Window.MainWindow != null)
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
}
