using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GenericRenderer.Assets
{
    public class RenderModel
    {
        public GCHandle MeshBufferHandle { get; set; }
        public List<RenderMesh> Meshes { get; set; } = new List<RenderMesh>();
        public Dictionary<string, RenderMaterial> Materials { get; set; } = new Dictionary<string, RenderMaterial>();
        public RenderSkeleton Skeleton { get; set; } 

        public void GenericDraw()
        {
            Skeleton.BufferData();

            foreach (RenderMesh mesh in Meshes)
            {
                if (mesh.Material != null)
                {
                    Skeleton.BindSkeleton(mesh.Material);
                    Globals.GlobalSceneBuffer.BindBufferToShader(mesh.Material);
                }

                mesh.GenericDraw();
            }
        }

        ~RenderModel()
        {
            MeshBufferHandle.Free();
        }
    }
}
