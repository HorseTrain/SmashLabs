using GenericRenderer.Assets;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmashEngine.IO.Loaders
{
    public struct ShaderPointer
    {
        public int ShaderType;
        public int NameOffset;
    }
    public class MaterialLoader
    {
        public static RenderMaterial LoadMaterial(CrossFile file)
        {
            RenderMaterial Out = new RenderMaterial();

            return Out;
        }

        public static unsafe RenderShader LoadShader(byte[] Data)
        {
            RenderShader Out = new RenderShader();

            GCHandle handler = CrossFile.CreateHandle(Data);

            CrossFile shaderfile = new CrossFile(CrossFile.GetArrayPointer(Data));

            shaderfile.Seek(shaderfile.Pointers[0].Offset);

            byte size = shaderfile.ReadObject<byte>();

            for (int i = 0; i < size; i++)
            {
                ShaderPointer pointer = shaderfile.ReadObject<ShaderPointer>();

                ShaderSource Source = new ShaderSource();

                Source.Type = (ShaderType)pointer.ShaderType;

                Source.Source = shaderfile.ReadString(shaderfile.Pointers[1].Offset + pointer.NameOffset);

                Out.Sources.Add(Source);
            }

            handler.Free();

            return Out;
        }

      
    }
}
