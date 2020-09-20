using SmashLabs.IO;
using SmashLabs.IO.Parsables.Model;
using SmashLabs.IO.Parsables.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabsTester
{
    class Program
    {
        static void Main(string[] args)
        {
            LEKS modelfile = (LEKS)IParsable.FromFile(@"C:\Users\Raymond\Desktop\SmashLabs\SmashLabs-master\SmashLabsTester\bin\Debug\test\c00\model.nusktb");

            modelfile.ExportData(@"D:\Games\Yuzu\sdmc\UltimateModManager\mods\Exporters\fighter\mario\model\body\c00\model.nusktb");

            Console.Read();
        }
    }
}
