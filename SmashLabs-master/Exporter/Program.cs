using SmashLabs.IO;
using SmashLabs.IO.Parsables.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exporter
{
    class Program
    {
        static void Main(string[] args)
        {
          // CrossFile temp = new CrossFile();

          //  Exporters.AddModelToCrossFile(temp, @"D:\Programming\Projects\Assets\fighter\mario\model\body\c00");

           // temp.Export("temp.cfile");

            Exporters.LoadAnimationFile((MINA)IParsable.FromFile(@"D:\Programming\Projects\Assets\fighter\mario\motion\body\c00\a00wait1.nuanmb")).Export("animation.cfile");
        }
    }
}
