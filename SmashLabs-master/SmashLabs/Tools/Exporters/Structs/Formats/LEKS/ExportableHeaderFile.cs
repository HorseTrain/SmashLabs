using SmashLabs.IO;
using SmashLabs.Structs;
using SmashLabs.Tools.Exporters.Structs.Formats.LDOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.Tools.Exporters.Structs.Formats.LEKS
{
    public struct ExportableHeaderFile
    {
        public HBSSHeader Header;
        public SmashFileMagic Magic;
        public long OffsetToBoneEntries;
        public ExportableMatrixPointer WorldTransformBuffer;
        public ExportableMatrixPointer InverseWorldTransform;
        public ExportableMatrixPointer LocalTransform;
        public ExportableMatrixPointer InverseLocalTransform;
        public long EntryCount;
    }
}
