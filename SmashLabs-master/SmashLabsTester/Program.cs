using SmashLabs.IO;
using SmashLabs.IO.Parsables.Mesh;
using SmashLabs.IO.Parsables.Model;
using SmashLabs.IO.Parsables.Skeleton;
using SmashLabs.Structs;
using SmashLabs.Tools.Accessors;
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
            LEKS skeleton = (LEKS)IParsable.FromFile(@"D:\Programming\ArcCross-master\CrossArc\bin\Debug\root\fighter\mario\model\body\c00\model.nusktb");

            skeleton.ExportData(@"D:\Games\Yuzu\sdmc\UltimateModManager\mods\Exporters\fighter\mario\model\body\c00\model.nusktb");

            HSEM meshfile = (HSEM)IParsable.FromFile(@"D:\Programming\ArcCross-master\CrossArc\bin\Debug\root\fighter\mario\model\body\c00\model.numshb");

            meshfile.ExportData(@"D:\Games\Yuzu\sdmc\UltimateModManager\mods\Exporters\fighter\mario\model\body\c00\model.numshb");

            Console.Read();
        }
    }
}
