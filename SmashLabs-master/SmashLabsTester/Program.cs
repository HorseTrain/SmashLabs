using SmashLabs.IO;
using SmashLabs.IO.Parsables.Model;
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
            LDOM modelfile = (LDOM)IParsable.FromFile(@"C:\Users\Raymond\Desktop\SmashLabs\SmashLabs-master\SmashLabsTester\bin\Debug\test\c00\model.numdlb");

            modelfile.ExportData(@"D:\Games\Yuzu\sdmc\UltimateModManager\mods\Exporters\fighter\mario\model\body\c00\model.numdlb");

            Console.Read();
        }
    }
}
