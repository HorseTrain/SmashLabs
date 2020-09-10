using SmashLabs.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.IO.Parsables.Material
{
    public class LTAM : IParsable
    {
        public MaterialEntry[] Entries { get; set; }

        public override void LoadData()
        {
            Entries = MaterialEntry.ParseEntries(reader);
        }
    }
}
