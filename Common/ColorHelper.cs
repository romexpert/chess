using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    static class ColorHelper
    {
        public static Color GetOpposite(Color color)
        {
            if(color == Color.None)
                throw new Exception("Color should be specified");

            return color == Color.White ? Color.Black : Color.White;
        }
    }
}
