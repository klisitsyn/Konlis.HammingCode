using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konlis.HammingCode
{
    public static class Util
    {
        public static string BoolArrayToString(bool[] encoded)
        {
            return string.Join("", encoded.Select(x => x ? "1" : "0"));
        }
    }
}
