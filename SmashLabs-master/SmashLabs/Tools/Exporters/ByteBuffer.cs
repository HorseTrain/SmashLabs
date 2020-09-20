using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.Tools.Exporters
{
    public unsafe class ByteBuffer : List<byte>
    {
        public void AddPadding(int size)
        {
            for (int i = 0; i < size; i++)
                Add(0);
        }

        public void AddObjectToArray<T>(T data) where T : unmanaged
        {
            byte* temp = (byte*)&data;

            for (int i = 0; i < sizeof(T); i++)
            {
                Add(temp[i]);
            }
        }

        public void AddObjectArrayToArray<T>(T[] data) where T : unmanaged
        {
            foreach (T temp in data)
            {
                AddObjectToArray(temp);
            }
        }

        public void InsertObjectToArray<T>(T data, int location) where T : unmanaged
        {
            byte* temp = (byte*)&data;

            for (int i = sizeof(T) - 1; i > -1; i--)
            {
                Insert(location, temp[i]);
            }
        }

        public void AddStringToBuffer(string str)
        {
            int size = 0;

            foreach (char c in str)
            {
                Add((byte)c);
                size++;
            }

            int toadd = 4 - (size % 4);

            if (toadd == 0)
                AddPadding(4);
            else
                AddPadding(toadd);
        }

        public static byte* GetDataPointer(byte[] Temp)
        {
            fixed (byte* temp = Temp)
            {
                return temp;
            }
        }
    }
}
