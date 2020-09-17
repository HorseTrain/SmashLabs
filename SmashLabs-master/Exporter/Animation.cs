using SmashLabs.IO.Parsables.Animation;
using SmashLabs.Tools.Accessors.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exporter
{
    public enum TrackType
    {
        Transform,
        Visibility,
        UNK
    }

    public struct AnimationNodePointer
    {
        public int NameOffset;
        public int DataOffset;
        public TrackType Type;
        public short FrameCount;
    }

    public static unsafe partial class Exporters
    {
        public static CrossFile LoadAnimationFile(MINA animation)
        {
            CrossFile Temp = new CrossFile();

            Temp.Magic = "ANIM";

            List<byte> NodePointers = Temp.AddBuffer();
            List<byte> DataBuffer = Temp.AddBuffer();

            AnimationTrackAccessor accessor = new AnimationTrackAccessor(animation);

            int c = 0;

            foreach (AnimationGroup group in animation.Animations)
            {
                foreach (AnimationNode node in group.Nodes)
                {
                    foreach (AnimationTrack track in node.Tracks)
                    {
                        AnimationNodePointer pointer = new AnimationNodePointer();

                        pointer.Type = TrackType.UNK;

                        switch (track.Name)
                        {
                            case "Transform": pointer.Type = TrackType.Transform; break;
                            case "Visibility": pointer.Type = TrackType.Visibility; break;
                        }

                        pointer.FrameCount = (short)track.FrameCount;
                        pointer.DataOffset = DataBuffer.Count;

                        object[] data = accessor.ReadTrack(track);

                        foreach (object o in data)
                        {
                            switch (track.Name)
                            {
                                case "Transform": CrossFile.AddObjectToArray((AnimationTransform)o, DataBuffer); break;
                                case "Visibility": CrossFile.AddObjectToArray((bool)o, DataBuffer); break;
                            }
                        }

                        pointer.NameOffset = DataBuffer.Count;
                        CrossFile.AddStringToBuffer(node.Name,DataBuffer);

                        CrossFile.AddObjectToArray(pointer,NodePointers);

                        c++;
                    }
                }
            }

            Temp.unk0 = (short)animation.FrameCount;
            Temp.unk1 = (short)c;

            return Temp;
        }
    }
}
