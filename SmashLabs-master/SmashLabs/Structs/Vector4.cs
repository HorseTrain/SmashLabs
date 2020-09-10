using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.Structs
{
    public struct Vector4
    {
        public float X, Y, Z, W;

        public Vector4(float X, float Y, float Z,float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + "," + W + ")";
        }
    }
}
