using SmashExporter.CrossExporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace SmashExporter
{
    public class ShaderSource
    {
        public int ShaderType { get; set; }
        public string Source { get; set; }
    }

    public struct ShaderPointer
    {
        public int ShaderType;
        public int NameOffset;
    }

    public partial class Exporters
    {
        public static CrossFile GetShaderFile(string path)
        {
            CrossFile Out = new CrossFile();

            Out.Magic = "SHADER";

            string[] paths = Directory.GetFiles(path);

            List<byte> PointerData = Out.AddBuffer();
            List<byte> StringBuffer = Out.AddBuffer();

            PointerData.Add((byte)Directory.GetFiles(path).Length);

            foreach (string p in paths)
            {
                ShaderPointer pointer = new ShaderPointer()
                {
                    ShaderType = int.Parse(Path.GetFileName(p).Split('.')[0]),
                    NameOffset = StringBuffer.Count
                };

                CrossFile.AddStringToBuffer(FixString(File.ReadAllText(p)),ref StringBuffer);

                CrossFile.AddObjectToBuffer(pointer,ref PointerData);
            }

            return Out;
        }

        public static string FixString(string In)
        {
            string Out = "";

            foreach (char c in In)
            {
                if (c != (char)13)
                Out += c;
            }

            return Out;
        }
    }
}
