using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exporter
{
    public unsafe struct FileHeader
    {
        public long Magic;
        public short BufferCount;
        public short unk0;
        public short unk1;

        public static long FromString(string str)
        {
            byte[] temp = new byte[8];

            for (int i = 0; i < temp.Length && i < str.Length;i++)
            {
                temp[i] = (byte)str[i];
            }

            fixed (byte* tmp = temp)
            {
                return *(long*)tmp;
            }
        }
    }

    public unsafe class CrossFile
    {
        public string Magic { get; set; } = "";
        public short unk0 { get; set; }
        public short unk1 { get; set; }
        public FileHeader header => new FileHeader() { Magic = FileHeader.FromString(Magic),unk0 = unk0,unk1 = unk1,BufferCount = (short)Buffers.Count};
        public List<byte> FinalBuffer { get; set; } = new List<byte>();
        public List<List<byte>> Buffers { get; set; } = new List<List<byte>>();

        public List<byte> AddBuffer()
        {
            List<byte> Out = new List<byte>();

            Buffers.Add(Out);

            return Out;
        }

        public void BuildBuffer(bool reset = true)
        {
            if (reset)
            {
                FinalBuffer = new List<byte>();
            }

            AddObjectToArray(header, FinalBuffer);

            for (int i = 0; i < Buffers.Count; i++)
            {
                InsertObjectToArray(Buffers[i].Count,Buffers[i],0);
            }

            foreach (List<byte> buffer in Buffers)
            {
                foreach (byte b in buffer)
                {
                    FinalBuffer.Add(b);
                }
            }
        }

        public void Export(string path)
        {
            BuildBuffer();

            File.WriteAllBytes(path,FinalBuffer.ToArray());
        }

        public static void AddObjectToArray<T>(T data,List<byte> Data) where T : unmanaged
        {
            byte* temp = (byte*)&data;

            for (int i = 0; i < sizeof(T); i++)
            {
                Data.Add(temp[i]);
            }
        }

        public static void AddObjectArrayToArray<T>(T[] data, List<byte> Data) where T : unmanaged
        {
            foreach(T temp in data)
            {
                AddObjectToArray(temp, Data);
            }
        }

        public static void InsertObjectToArray<T>(T data, List<byte> Data,int location) where T : unmanaged
        {
            byte* temp = (byte*)&data;

            for (int i = sizeof(T) - 1; i > -1; i--)
            {
                Data.Insert(location,temp[i]);
            }
        }

        public static void AddStringToBuffer(string str, List<byte> Data)
        {
            foreach (char c in str)
            {
                Data.Add((byte)c);
            }

            Data.Add(0);
        }
    }
}
