using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashEngine.Graphics.MaterialAttributes
{
    public enum WrapMode : int
    {
        Repeat = 0,
        ClampToEdge = 1,
        MirroredRepeat = 2,
        ClampToBorder = 3
    }
}
