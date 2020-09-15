using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SmashEngine.IO
{
    public unsafe struct FilePointer
    {
        public byte* Location;
        public long Offset;
    }

    public struct FileHeader
    {
        public long FileMagic;
        public int Unk0;
        public int FileCount;
    }

    public unsafe class CrossFile
    {
        public FilePointer[] Pointers;
        public byte* Data { get; private set; }
        public FileHeader Header => *((FileHeader*)Data);
        public long BufferLocation { get; private set; }

        public CrossFile(void* Data)
        {
            this.Data = (byte*)Data;

            Pointers = new FilePointer[Header.FileCount];

            Seek(sizeof(FileHeader));

            for (int i = 0; i < Header.FileCount; i++)
            {
                Pointers[i] = new FilePointer()
                {
                    Location = &this.Data[BufferLocation + 8],
                    Offset = BufferLocation + 8
                };

                Advance(ReadObject<long>());
            }
        }

        public void Advance(long size)
        {
            BufferLocation += size;
        }

        public void Seek(long offset)
        {
            BufferLocation = offset;
        }

        public T ReadObject<T>() where T : unmanaged
        {
            T Out = *(T*)&Data[BufferLocation];

            Advance(sizeof(T));

            return Out;
        }

        public T[] ReadObject<T>(long count) where T : unmanaged
        {
            T[] Out = new T[count];

            for (int i = 0; i < Out.Length; i++)
            {
                Out[i] = ReadObject<T>();
            }

            return Out;
        }

        public T ReadObjectDirect<T>(long offset) where T : unmanaged
        {
            long temp = BufferLocation;

            Seek(offset);

            T Out = ReadObject<T>();

            Seek(temp);

            return Out;
        }

        public string ReadString(long offset)
        {
            string Out = "";

            long last = BufferLocation;

            Seek(offset);

            Out = ReadString();

            Seek(last);

            return Out;
        }

        public string ReadString()
        {
            string Out = "";

            while (true)
            {
                byte temp = ReadObject<byte>();

                if (temp == 0)
                    break;
                else
                    Out += (char)temp;
            }

            return Out;
        }

        public static GCHandle CreateHandle(object data,GCHandleType type = GCHandleType.Pinned)
        {
            return GCHandle.Alloc(data, type);
        }

        public static T* GetArrayPointer<T>(T[] Data) where T: unmanaged
        {
            fixed (T* Out = Data)
            {
                return Out;
            }
        }
    }
}
