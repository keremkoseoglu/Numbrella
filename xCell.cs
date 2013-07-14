using System;
using System.Collections.Generic;
using System.Text;

namespace Numbrella
{
    public class xCell
    {
        public enum SOURCE : byte { UNKNOWN, QUESTION, COMPUTER, HUMAN, BRUTE };

        public byte value;
        public SOURCE source;
        public bool error;

        public xCell()
        {
            value = 0;
            source = SOURCE.UNKNOWN;
            error = false;
        }

        public void copyTo(ref xCell Cell)
        {
            Cell.value = value;
            Cell.source = source;
            Cell.error = error;
        }
    }
}
