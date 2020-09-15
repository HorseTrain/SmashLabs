using System.IO;
using System.Drawing;
using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace GenericRenderer.Assets
{
    public class RenderTexture
    {
        public int Handle { get;private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        internal RenderTexture(int Width,int Height,int Handle)
        {
            this.Width = Width;
            this.Height = Height;
            this.Handle = Handle;
        }
        public static unsafe Bitmap BitmapFromPointer(byte* Data,long size)
        {
            Bitmap bmp;
            using (var ms = new UnmanagedMemoryStream(Data,size))
            {
                bmp = new Bitmap(ms);
            }

            return bmp;
        }

        public RenderTexture(Bitmap map)
        {
            CreateTexture(map);
        }

        public void CreateTexture(Bitmap image)
        {
            Handle = GL.GenTexture();
            Width = image.Width;
            Height = image.Height;
            Use();
            using (image)
            {
                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);
            }
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 9728);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 9728);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, 10497);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, 10497);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            if (this.Handle == -1)
                return;
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, this.Handle);
        }
    }
}
