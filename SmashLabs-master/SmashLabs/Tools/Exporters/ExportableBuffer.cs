using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.Tools.Exporters
{
    public unsafe class ExportableBuffer
    {
        public List<List<byte>> Buffers { get; set; } = new List<List<byte>>();

        public long CurrentSize
        {
            get
            {
                long Out = 0;

                foreach (List<byte> buffer in Buffers)
                {
                    Out += buffer.Count;
                }

                return Out;
            }
        }

        public ByteBuffer AddBuffer()
        {
            ByteBuffer Temp = new ByteBuffer();

            Buffers.Add(Temp);

            return Temp;
        }

        public List<byte> BuildFinalBuffer()
        {
            List<byte> Out = new List<byte>();

            foreach (List<byte> buffer in Buffers)
            {
                foreach (byte b in buffer)
                {
                    Out.Add(b);
                }
            }

            return Out;
        }
    }
}
