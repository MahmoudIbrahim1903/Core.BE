using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Utilities
{
    public static class MathUtility
    {
        public static decimal Round(decimal value, int decimalPlaces) => Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);

    }
}
