using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CrossProject.CoreEngine.Graphics
{
    public class Window
    {
        public static Window MainWindow;

        public GameWindow glWindow { get; private set; }
        public int Width => glWindow.Width;
        public int Height => glWindow.Height;
        public float Aspect => (float)Width / (float)Height;

        public static void CreateNewWindow(int Width, int Height)
        {
            Window Out = new Window();

            MainWindow = Out;

            Out.glWindow = new GameWindow(Width,Height);

            Out.glWindow.UpdateFrame += Out.MainLoop;

            Out.glWindow.Run();
        }

        public void MakeCurrent()
        {
            glWindow.MakeCurrent();
        }

        public void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.2f,0.2f,0.2f,1);
        }

        public void MainLoop(object sender, FrameEventArgs args)
        {
            Clear();

            glWindow.SwapBuffers();
        }
    }
}
