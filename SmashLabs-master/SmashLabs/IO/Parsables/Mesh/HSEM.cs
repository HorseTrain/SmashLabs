using SmashLabs.IO.Parsables.Mesh.Rigging;
using SmashLabs.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashLabs.IO.Parsables.Mesh
{
    public unsafe class HSEM : IParsable
    {
        public string Name { get; set; }
        MeshMetaData MeshData { get; set; }
        public int[] BufferSizes { get; set; }
        public long PolygonIndexSize { get; set; }
        public MeshBuffer[] VertexBuffers { get; set; }
        public MeshObject[] Objects { get; set; }
        public PolygonBuffer Polygonbuffer { get; set; }
        public MeshRiggingGroup[] RigBuffers { get; set; }
        public override void LoadData()
        {
            Name = reader.ReadStringOffset();

            MeshData = reader.ReadObject<MeshMetaData>();

            Objects = MeshObject.ReadObjects(reader);

            ReadBufferSizes();

            PolygonIndexSize = reader.ReadLong();

            VertexBuffers = MeshBuffer.ParseMeshBuffers(reader);

            Polygonbuffer = PolygonBuffer.ParsePolygonBuffer(reader);

            RigBuffers = MeshRiggingGroup.ParseMeshRiggingGroups(reader);
        }

        void ReadBufferSizes()
        {
            BufferArrayPointer pointer = reader.ReadArrayPointer();

            BufferSizes = reader.ReadObject<int>((int)pointer.ElementCount);

            pointer.End();
        }


    }
}
