using GenericRenderer;
using GenericRenderer.Assets;
using GenericRenderer.Game;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SmashEngine.Graphics.Animation;
using SmashEngine.IO;
using SmashEngine.IO.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmashEngine
{
    public class Program
    {
        public static RenderModel model;
        public static RenderAnimation animation;

        static unsafe void Main(string[] args)
        {
            Game window = new Game();

SmashExporter.Exporters.GetModelFile(@"D:\Programming\Projects\Assets\fighter\mario\model\body\c00").Export(@"C:\Users\Raymond\Desktop\Assets\Extracted\mario.cfile");

            SmashExporter.Exporters.GetAnimationFile(@"D:\Programming\Projects\Assets\fighter\mario\motion\body\c00\a00wait1.nuanmb").Export(@"C:\Users\Raymond\Desktop\Assets\Extracted\anim.cfile");

            SmashExporter.Exporters.GetShaderFile(@"C:\Users\Raymond\Desktop\Assets\Shaders\SmashModel\").Export(@"C:\Users\Raymond\Desktop\Assets\Extracted\test.shader");

            window.CreateWindow(500,500);
        }
    }

    public class Game : Window
    {
        public override void MainLoop(object sender, FrameEventArgs args)
        {
            PreWindow();

            Globals.Update();

            if (Program.model == null)
            {
                RenderShader.ShaderCollection.Add("Fighter", MaterialLoader.LoadShader(File.ReadAllBytes(@"C:\Users\Raymond\Desktop\Assets\Extracted\test.shader")));

                Program.model = RenderModelLoader.LoadModel(File.ReadAllBytes(@"C:\Users\Raymond\Desktop\Assets\Extracted\mario.cfile"));
                Program.animation = AnimationLoader.LoadAnimation(File.ReadAllBytes(@"C:\Users\Raymond\Desktop\Assets\Extracted\anim.cfile"));
            }

            foreach (AnimationNode node in Program.animation.Nodes)
            {
                foreach (AnimationTrack track in node.Tracks)
                {
                    if (track.Type == AnimationTrackType.Transform)
                    {
                        BoneNode bone = Program.model.Skeleton.GetNode(node.Name);

                        if (bone != null)
                        {
                            bone.LocalRotation = track.GetKey<TransformKey>(0).Rotation;
                        }
                    }
                }
            }

            Program.model.GenericDraw();



            PostWindow();

            Console.Read();
        }
    }
}
