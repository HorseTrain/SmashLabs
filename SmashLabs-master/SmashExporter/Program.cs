using OpenTK.Graphics.OpenGL;
using SmashExporter.CrossExporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace SmashExporter
{
    public class Program
    {
        public static void Main()
        {
            Exporters.GetModelFile(@"D:\Programming\Projects\Assets\fighter\mario\model\body\c00").Export("mario.cfile");

            Exporters.GetShaderFile(@"C:\Users\Raymond\Desktop\Assets\Shaders\SmashModel\").Export("test.shader");

            Console.WriteLine("smh");
        }
    }
}
