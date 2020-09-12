using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericRenderer.Structs
{
    public struct RenderVertex
    {
        public Vector3 VertexPosition;
        public Vector3 VertexNormal;
        public Vector2 VertexMap0;
        public Vector2 VertexMap1;
        public Vector4 VertexColor;
        public Vector4I VertexWeightIndex;
        public Vector4 VertexWeight;
    }
}
