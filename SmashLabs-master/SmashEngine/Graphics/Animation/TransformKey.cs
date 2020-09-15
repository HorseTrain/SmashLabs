using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashEngine.Graphics.Animation
{
    public struct TransformKey
    {
        public Vector3 Scale;
        public Quaternion Rotation;
        public Vector3 Position;
        public float CompensateScale; //?

        public override string ToString()
        {
            return Scale + "\n" + Rotation + "\n" + Position + "\n" + CompensateScale;
        }
    }
}
