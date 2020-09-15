using GenericRenderer.Assets;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericRenderer
{
    public class Globals
    {
        public static UniformBuffer<Matrix4> GlobalSceneBuffer { get; set; }
        public static RenderCamera MainCamera { get; set; } = new RenderCamera();
        public static void SetUp()
        {
            GlobalSceneBuffer = new UniformBuffer<Matrix4>(1000, "SceneTransforms");
        }

        public static void Update()
        {
            GlobalSceneBuffer[0] = MainCamera.CameraViewThrusum;

            GlobalSceneBuffer.BufferData();
        }
    }
}
