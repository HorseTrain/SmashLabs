using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.Structs
{
    public struct Matrix4
    {
        public Vector4 Row0;
        public Vector4 Row1;
        public Vector4 Row2;
        public Vector4 Row3;

        public override string ToString()
        {
            return 
                Row0.ToString() + "\n" +
                Row1.ToString() + "\n" +
                Row2.ToString() + "\n" +
                Row3.ToString();
        }
    }
}
