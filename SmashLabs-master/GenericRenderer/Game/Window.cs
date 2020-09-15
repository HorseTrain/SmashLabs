using OpenTK.Graphics.OpenGL;
using OpenTK;
using System;

namespace GenericRenderer.Game
{
    public class Window
    {
        public static Window MainWindow { get; private set; } = null;
        public GameWindow glWindow { get; private set; }
        public int Width { get => glWindow.Width; set { glWindow.Width = value; } }
        public int Height { get => glWindow.Height; set { glWindow.Height = value; } }
        public float ScreenAspect => (float)Width / Height;
        public bool Active { get; private set; }

        public void CreateWindow(int width, int height)
        {
            if (MainWindow == null)
            {
                glWindow = new GameWindow(width, height);

                glWindow.UpdateFrame += MainLoop;

                MainWindow = this;

                Active = true;

                GL.Enable(EnableCap.DepthTest);

                Globals.SetUp();

                glWindow.Run();

                Active = false;
            }
        }
       
        public virtual void MainLoop(object sender, FrameEventArgs args)
        {
            PreWindow();

            PostWindow();
        }

        public void PreWindow()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);;
            GL.ClearColor(0.2f,0.2f,0.2f,1);

            GL.Viewport(0,0,Width,Height);
        }

        public void PostWindow()
        {
            glWindow.SwapBuffers();
        }
    }
}
