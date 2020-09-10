using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.IO
{
    public struct SmashFileMagic
    {
        public byte Magic0, Magic1, Magic2, Magic3;
        public short MajorVersion; //?
        public short MinorVersion; //?

        public override string ToString()
        {
            string Out = "";

            Out += (char)Magic0;
            Out += (char)Magic1;
            Out += (char)Magic2;
            Out += (char)Magic3;

            return Out;
        }
    }
}
