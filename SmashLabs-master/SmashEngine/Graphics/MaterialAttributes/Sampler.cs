using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashEngine.Graphics.MaterialAttributes
{
    public struct Sampler
    {
        public WrapMode WrapS;
        public WrapMode WrapT;
        public WrapMode WrapR;
        public int MinFilter;
        public int MagFilter;
        public int Unk6;
        public int Unk7;
        public int Unk8;
        public int Unk9;
        public int Unk10;
        public int Unk11;
        public int Unk12;
        public float LodBias;
        public int MaxAnisotropy;
    }
}
