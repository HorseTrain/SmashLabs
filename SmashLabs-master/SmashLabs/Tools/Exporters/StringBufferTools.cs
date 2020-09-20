using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.Tools.Exporters
{
    public class StringBufferPointer
    {
        public string Data { get; set; }
        public long PointerLocation { get; set; }
        public int StringOffset { get; set; }
        public StringBufferPointer(string data,long pointerlocation)
        {
            Data = data;
            PointerLocation = pointerlocation;
        }
    }

    public class StringBuffer : List<StringBufferPointer>
    {
        public void AddString(string source,long location)
        {
            StringBufferPointer pointer = new StringBufferPointer(source,location);

            Add(pointer);
        }
    }
}
