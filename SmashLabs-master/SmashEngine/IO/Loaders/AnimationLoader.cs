using GenericRenderer.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashEngine.IO.Loaders
{
    public struct AnimationNodePointer
    {
        public int NameOffset;
        public byte AnimationTrackCount;
        public int AnimationTrackOffset;
    }
    public struct AnimationTrackPointer
    {
        public short FrameCount;
        public int DataOffset;
        public AnimationTrackType type;
    }
    public static unsafe class AnimationLoader
    {
        public static RenderAnimation LoadAnimation(byte[] Data)
        {
            RenderAnimation Out = new RenderAnimation();

            Out.BufferHandle = CrossFile.CreateHandle(Data);

            CrossFile mainreader = new CrossFile((char*)CrossFile.GetArrayPointer(Data));

            mainreader.Seek(mainreader.Pointers[0].Offset);

            AnimationTrackPointer* Pointers = (AnimationTrackPointer*)mainreader.Pointers[1].Location;

            Out.Nodes = new List<AnimationNode>();

            for (int i = 0; i < mainreader.Header.Unk0; i++)
            {
                AnimationNodePointer pointer = mainreader.ReadObject<AnimationNodePointer>();

                AnimationNode node = new AnimationNode();

                node.Name = mainreader.ReadString(pointer.NameOffset + mainreader.Pointers[3].Offset);

                long lp = mainreader.BufferLocation;

                mainreader.Seek(pointer.AnimationTrackOffset + mainreader.Pointers[1].Offset);

                for (int t = 0; t < pointer.AnimationTrackCount; t++)
                {
                    AnimationTrackPointer tpointer = mainreader.ReadObject<AnimationTrackPointer>();

                    AnimationTrack track = new AnimationTrack();

                    track.Type = tpointer.type;
                    track.Data = &mainreader.Data[tpointer.DataOffset + mainreader.Pointers[2].Offset];
                    track.FrameCount = tpointer.FrameCount;

                    node.Tracks.Add(track);
                }

                mainreader.Seek(lp);

                Out.Nodes.Add(node);
            }

            return Out;
        }
    }
}
