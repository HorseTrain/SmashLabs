using SmashLabs.IO.Parsables.Skeleton;
using SmashLabs.Structs;
using SmashLabs.Tools.Exporters;
using SmashLabs.Tools.Exporters.Structs;
using SmashLabs.Tools.Exporters.Structs.Formats.LDOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.IO.Parsables.Model
{
    public class LDOM : IParsable
    {
        public string Name { get; set; }
        public string SkeletonFileName { get; set; }
        public string[] MaterialFileNames { get; set; }
        public string UnknownFileName { get; set; } 
        public string MeshCollectionFileName { get; set; }
        public ModelEntry[] Entries { get; set; }

        public override void LoadData()
        {
            Name = reader.ReadStringOffset();
            SkeletonFileName = reader.ReadStringOffset();

            MaterialFileNames = reader.ReadStringArray();

            UnknownFileName = reader.ReadStringOffset();
            MeshCollectionFileName = reader.ReadStringOffset();

            BufferArrayPointer entrypointers = reader.ReadArrayPointer();
            Entries = new ModelEntry[entrypointers.ElementCount];

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
                SubIndex = reader.ReadLong(),
                MaterialName = reader.ReadStringOffset()
            };
        }

        public unsafe override byte[] GetData()
        {
            ExportableBuffer Exporter = new ExportableBuffer();

            ByteBuffer header = Exporter.AddBuffer();
            ByteBuffer headernames = Exporter.AddBuffer();

            ByteBuffer materialnamepointers = Exporter.AddBuffer();
            ByteBuffer materialnamebuffers = Exporter.AddBuffer();

            ByteBuffer pointerbuffer = Exporter.AddBuffer();
            ByteBuffer StringBuffer = Exporter.AddBuffer();

            LDOMExportableHeaderFile TempHeader = new LDOMExportableHeaderFile();

            { //Header Data
                TempHeader.Header = Header;
                TempHeader.Magic = Magic;

                TempHeader.NameOffset = sizeof(LDOMExportableHeaderFile) - (int)Marshal.OffsetOf(typeof(LDOMExportableHeaderFile), "NameOffset");
                headernames.AddStringToBuffer(Name);

                TempHeader.SkeletonNameOffset = (sizeof(LDOMExportableHeaderFile) - (int)Marshal.OffsetOf(typeof(LDOMExportableHeaderFile), "SkeletonNameOffset")) + headernames.Count;
                headernames.AddStringToBuffer(SkeletonFileName);

                TempHeader.UnknownFileNameOffset = 0;

                TempHeader.MeshFileNameOffset = (sizeof(LDOMExportableHeaderFile) - (int)Marshal.OffsetOf(typeof(LDOMExportableHeaderFile), "MeshFileNameOffset")) + headernames.Count;
                headernames.AddStringToBuffer(MeshCollectionFileName);
            }

            { //Material Names
                TempHeader.MaterialNamesPointer.Offset = (sizeof(LDOMExportableHeaderFile) + headernames.Count) - (int)Marshal.OffsetOf(typeof(LDOMExportableHeaderFile), "MaterialNamesPointer");
                TempHeader.MaterialNamesPointer.ElementCount = MaterialFileNames.Length;

                List<long> materialnameoffsets = new List<long>();

                foreach (string name in MaterialFileNames)
                {
                    materialnameoffsets.Add(materialnamebuffers.Count);
                    materialnamebuffers.AddStringToBuffer(name);
                }

                int endoffset = materialnameoffsets.Count * sizeof(long);

                for (int i = 0; i < materialnameoffsets.Count; i++)
                {
                    materialnamepointers.AddObjectToArray(endoffset + materialnameoffsets[i]);

                    endoffset -= 8;
                }
            }

            TempHeader.EntryPointers.ElementCount = Entries.Length;
            TempHeader.EntryPointers.Offset = (sizeof(LDOMExportableHeaderFile) + headernames.Count + materialnamepointers.Count + materialnamebuffers.Count) - (int)Marshal.OffsetOf(typeof(LDOMExportableHeaderFile), "EntryPointers");

            header.AddObjectToArray(TempHeader);

            long pointersize = 24 * Entries.Length;
            long stringoffset = pointersize;

            for (int i = 0; i < Entries.Length; i++)
            {
                ExportableModelEntry tempentry = new ExportableModelEntry();

                tempentry.SubIndex = Entries[i].SubIndex;

                tempentry.NameOffset = pointersize + StringBuffer.Count;
                StringBuffer.AddStringToBuffer(Entries[i].Name);

                tempentry.MaterialOffset = pointersize + StringBuffer.Count - (long)Marshal.OffsetOf(typeof(ExportableModelEntry), "MaterialOffset");
                StringBuffer.AddStringToBuffer(Entries[i].MaterialName) ;
        
                pointersize -= 24;

                pointerbuffer.AddObjectToArray(tempentry);
            }



            return Exporter.BuildFinalBuffer().ToArray();
        }
    }
}
