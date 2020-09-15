using SmashExporter.CrossExporter;
using SmashLabs.IO;
using SmashLabs.IO.Parsables.Animation;
using SmashLabs.Tools.Accessors.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmashExporter
{
    public unsafe partial class Exporters
    {
        public enum AnimationTrackType : byte
        {
            Transform,
            Visibility,
            Unk 
        }
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

        public static CrossFile GetAnimationFile(string path)
        {
            CrossFile Out = new CrossFile();

            MINA file = (MINA)IParsable.FromFile(path);

            Out.Magic = "ANIM";

            List<byte> NodePointers = Out.AddBuffer();
            List<byte> TrackPointers = Out.AddBuffer();
            List<byte> DataBuffer = Out.AddBuffer();
            List<byte> StringBuffer = Out.AddBuffer();

            AnimationTrackAccessor accessor = new AnimationTrackAccessor(file);

            int c = 0;

            foreach (AnimationGroup group in file.Animations)
            {
                foreach (AnimationNode node in group.Nodes)
                {
                    c++;

                    AnimationNodePointer npointer = new AnimationNodePointer();

                    npointer.AnimationTrackOffset = TrackPointers.Count;
                    npointer.AnimationTrackCount = (byte)node.Tracks.Length;

                    npointer.NameOffset = StringBuffer.Count;
                    CrossFile.AddStringToBuffer(node.Name,ref StringBuffer);

                    foreach (AnimationTrack track in node.Tracks)
                    {
                        AnimationTrackPointer tpointer = new AnimationTrackPointer();

                        tpointer.DataOffset = DataBuffer.Count;

                        object[] Data = accessor.ReadTrack(track);

                        tpointer.FrameCount = (short)Data.Length;

                        foreach (object o in Data)
                        {
                            switch (track.Name)
                            {
                                case "Transform": CrossFile.AddObjectToBuffer((AnimationTransform)o, ref DataBuffer); tpointer.type = AnimationTrackType.Transform; break;
                                case "Visibility": CrossFile.AddObjectToBuffer((bool)o, ref DataBuffer); tpointer.type = AnimationTrackType.Visibility; break;
                            }
                        }

                        CrossFile.AddObjectToBuffer(tpointer, ref TrackPointers);
                    }

                    CrossFile.AddObjectToBuffer(npointer,ref NodePointers);
                }
            }

            Out.Unk0 = c;

            return Out;
        }
    }
}
