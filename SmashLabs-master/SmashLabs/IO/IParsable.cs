using SmashLabs.IO.Parsables;
using SmashLabs.Tools;
using SmashLabs.IO.Parsables.Model;
using SmashLabs.IO.Parsables.Skeleton;
using SmashLabs.IO.Parsables.Mesh;
using SmashLabs.IO.Parsables.Material;
using SmashLabs.IO.Parsables.Animation;

namespace SmashLabs.IO
{
    public class IParsable
    {
        public BufferReader reader;

        public SmashFileMagic Magic { get; private set; }

        protected IParsable()
        {

        }

        public static IParsable FromFile(HBSSFile file,bool LoadData = true)
        {
            IParsable Out = null;

            BufferReader reader = new BufferReader(file.Buffer);

            reader.Seek(0x10);

            SmashFileMagic magic = reader.ReadObject<SmashFileMagic>();

            switch (magic.ToString())
            {
                case "LDOM": Out = new LDOM(); break;
                case "LEKS": Out = new LEKS(); break;
                case "HSEM": Out = new HSEM(); break;
                case "LTAM": Out = new LTAM(); break;
                case "MINA": Out = new MINA(); break;
            }

            if (Out == null)
                Out = new IParsable();

            Out.Magic = magic;

            Out.reader = reader;

            Out.LoadData();

            return Out;
        }

        public static IParsable FromFile(string path,bool LoadData = true)
        {
            return FromFile(HBSSFile.TryParseFile(path),LoadData);
        }
        public static IParsable FromFile(byte[] buffer, bool LoadData = true)
        {
            return FromFile(HBSSFile.TryParseFile(buffer), LoadData);
        }

        public virtual void LoadData() //Reader always starts at 0x18
        {

        }
    }
}
