using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossProject.CoreEngine.Physics.Lines
{
    public struct Line
    {
        public Vector2 Offset;
        public Vector2 Direction;
        public float T;

        public Vector2 GetPoint(float t)
        {
            return Offset + (Direction * t);
        }

        public static Line From2Points(Vector2 Point0,Vector2 Point1)
        {
            Line Out = new Line();

            Out.Offset = Point0;

            Vector2 Direction = Point1 - Point0;

            float len = Direction.Length;

            Out.Direction = Direction;
            Out.T = 1;

            return Out;
        }

        public float GetT(Vector2 point)
        {
            return (-Offset.X + point.X) / Direction.X; //Does not account for the point not being on the line.
        }

        public override string ToString()
        {
            return "{" + Offset + "," + Direction + "," + T + "}";
        }

        public LineSlope Slope
        {
            get
            {
                LineSlope Out = new LineSlope();

                Out.Slope = new Vector2(Direction.Y,-Direction.X);
                Out.Intercept = Vector2.Dot(Out.Slope,Offset);

                return Out;
            }
        }
    }
}
