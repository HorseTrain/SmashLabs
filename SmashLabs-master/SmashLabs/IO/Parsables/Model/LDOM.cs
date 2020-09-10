using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.IO.Parsables.Model
{
    public class LDOM : IParsable
    {
        public string Name { get; set; }
        public string SkeletonFileName { get; set; }
        public string UnknownFileName { get; set; } //Usually empty?
        public string MaterialFileName { get; set; } //Not implamented
        public string MeshCollectionFileName { get; set; }

        public ModelEntry[] Entries { get; set; }

        public override void LoadData()
        {
            Name = reader.ReadStringOffset();
            SkeletonFileName = reader.ReadStringOffset();
            reader.Advance(24); //I think this is material data?
            MeshCollectionFileName = reader.ReadStringOffset();

            long DataOffst = reader.ReadOffset();

            Entries = new ModelEntry[reader.ReadLong()];
            reader.Seek(DataOffst);

            for (int i = 0; i < Entries.Length; i++)
            {
                Entries[i] = ReadEntry();
            }
        }

        ModelEntry ReadEntry()
        {
            return new ModelEntry()
            {
                Name = reader.ReadStringOffset(),
                Unknown0 = reader.ReadLong(), //Figure out what this is
                MaterialName = reader.ReadStringOffset()
            };
        }
    }
}
