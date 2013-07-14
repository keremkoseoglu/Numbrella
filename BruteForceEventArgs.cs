using System;
using System.Collections.Generic;
using System.Text;

namespace Numbrella
{
    public class BruteForceEventArgs : EventArgs
    {
        public int level;
        public xBoard board;

        public BruteForceEventArgs(int Level, xBoard Board)
        {
            level = Level;
            board = Board;
        }
    }
}
