using SmashLabs.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.Tools.Exporters.Structs.Formats.LDOM
{
    public struct LDOMExportableHeaderFile
    {
        public HBSSHeader Header;
        public SmashFileMagic Magic;
        public long NameOffset;
        public long SkeletonNameOffset;
        public ExportableArrayPointer MaterialNamesPointer;
        public long UnknownFileNameOffset;
        public long MeshFileNameOffset;
        public ExportableArrayPointer EntryPointers;
    }
}
