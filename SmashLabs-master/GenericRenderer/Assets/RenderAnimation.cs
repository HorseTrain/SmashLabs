using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GenericRenderer.Assets
{
    public enum AnimationTrackType : byte
    {
        Transform,
        Visibility
    }

    public class AnimationNode
    {
        public string Name;

        public List<AnimationTrack> Tracks = new List<AnimationTrack>();
    }

    public unsafe class AnimationTrack
    {
        public AnimationTrackType Type;
        public void* Data; //Make sure to pin your data!!
        public short FrameCount;

        public T GetKey<T>(int index) where T : unmanaged
        {
            if (index < FrameCount)
                return ((T*)Data)[index];
            else
                return new T();
        }
    }

    public class RenderAnimation
    {
        public GCHandle BufferHandle;

        public List<AnimationNode> Nodes { get;set; }

        ~RenderAnimation()
        {
            BufferHandle.Free();
        }
    }
}
