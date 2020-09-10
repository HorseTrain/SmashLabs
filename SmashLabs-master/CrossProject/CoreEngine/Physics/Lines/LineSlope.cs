using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossProject.CoreEngine.Physics.Lines
{
    public struct LineSlope
    {
        public float Intercept;
        public Vector2 Slope;

        public override string ToString()
        {
            return Slope.X + "x+" + Slope.Y + "y=" + Intercept;
        }

        public static string WriteOutRelationship(LineSlope Line0,LineSlope Line1)
        {
            return Line0.Slope.X + "x+" + Line0.Slope.Y + "y-" + Line0.Intercept + "=" + Line1.Slope.X + "x+" + Line1.Slope.Y + "y-" + Line1.Intercept;
        }
    }
}
