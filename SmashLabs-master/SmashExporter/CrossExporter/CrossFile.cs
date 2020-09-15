using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SmashExporter.CrossExporter
{
    public struct FileHeader
    {
        public long FileMagic;
        public int Unk0;
        public int FileCount;
    }
    public struct BufferHeader
    {
        public long Offset;
        public int Size;
    }
    public unsafe class CrossFile
    {
        public string Magic = "";
        public int Unk0 = 0;
        public FileHeader header => new FileHeader() { FileMagic = FromString(Magic),Unk0 = Unk0,FileCount = Buffers.Count };
        public List<byte> FinalBuffer = new List<byte>();

        public List<List<byte>> Buffers = new List<List<byte>>(); 
     
        public List<byte> AddBuffer()
        {
            Buffers.Add(new List<byte>());

            return Buffers[Buffers.Count - 1];
        }

        public unsafe long FromString(string str)
        {
            byte[] temp = new byte[8];

            for (int i = 0; i < 8 && i < str.Length; i++)
            {
                temp[i] = (byte)str[i];
            }

            fixed(byte* t = temp)
            {
                return *((long*)t);
            }
        }
        public unsafe static void AddObjectToBuffer<T>(T Object, ref List<byte> Data) where T: unmanaged
        {
            byte* temp = (byte*)&Object;

            for (int i = 0; i < sizeof(T); i++)
                Data.Add(temp[i]);
        }

        public unsafe static void InsertObjectToBuffer<T>(T Object, ref List<byte> Data,int index) where T : unmanaged
        {
            byte* temp = (byte*)&Object;

            for (int i = 0; i < sizeof(T); i++)
                Data.Insert(index,temp[sizeof(T) - i - 1]);
        }
        public static void AddArrayToObject<T>(T[] Object, ref List<byte> Data) where T : unmanaged
        {
            foreach (T o in Object)
            {
                AddObjectToBuffer(o, ref Data);
            }
        }
        public static void AddStringToBuffer(string str,ref List<byte> Data)
        {
            foreach (char c in str)
            {
                Data.Add((byte)c);
            }

            Data.Add(0);
        }
        void Build(bool reset = true)
        {
            if (reset)
            FinalBuffer = new List<byte>();

            foreach (List<byte> buffers in Buffers)
            {
                AddObjectToBuffer((long)(buffers.Count), ref FinalBuffer);

                foreach (byte b in buffers)
                {
                    FinalBuffer.Add(b);
                }
            }
        }

        public void BuildFile()
        {
            InsertObjectToBuffer(header, ref FinalBuffer, 0);

            Build(false);
        }

        public void Export(string path)
        {
            BuildFile();

            File.WriteAllBytes(path,FinalBuffer.ToArray());
        }
    }
}
