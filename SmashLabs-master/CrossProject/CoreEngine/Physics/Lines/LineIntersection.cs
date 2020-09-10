using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossProject.CoreEngine.Physics.Lines
{
    public class LineIntersection
    {
        public Line Line0;
        public Line Line1;
        public bool Valid;
        public Vector2 IntersectionPoint;

        public static LineIntersection TestIntersection(Line line0, Line line1)
        {
            LineIntersection Out = new LineIntersection();

            LineSlope slope = line1.Slope;

            float t = (-(slope.Slope.X * line0.Offset.X) - (slope.Slope.Y * line0.Offset.Y) + slope.Intercept) / ((slope.Slope.X * line0.Direction.X) + (slope.Slope.Y * line0.Direction.Y));

            Out.IntersectionPoint = line0.GetPoint(t);

            Out.Line0 = line0;
            Out.Line1 = line1;

            float t1 = line1.GetT(Out.IntersectionPoint);

            Out.Valid = t1 >= 0 && t1 <= 1 && t >= 0 && t <= 1;

            return Out;
        }

        public override string ToString()
        {
            return Line0.ToString() + Line1.ToString() + " " + Valid.ToString() + " " + IntersectionPoint.ToString();
        }
    }
}
