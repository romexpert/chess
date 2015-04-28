using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class RequireExchangeException : Exception
    {
        public Color Color { get; set; }
    }
}