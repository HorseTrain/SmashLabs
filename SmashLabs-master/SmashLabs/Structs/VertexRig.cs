using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.Structs
{
    public struct VertexRig
    {
        public Vector4I VertexWeightIndex;
        public Vector4 VertexWeight;

        public override string ToString()
        {
            return VertexWeightIndex.ToString() + "\n" + VertexWeight.ToString() + "\n";
        }
    }
}
