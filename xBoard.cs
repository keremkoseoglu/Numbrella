using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Numbrella
{
    public class xBoard
    {
        public xCell[,] cells;

        public xBoard()
        {
            refresh();
        }

        public void copyTo(ref xBoard Board)
        {
            for (byte x = 0; x < 9; x++) for (byte y = 0; y < 9; y++) cells[x, y].copyTo(ref Board.cells[x, y]);
        }

        private void refresh()
        {
            cells = new xCell[9, 9];

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    cells[x, y] = new xCell();
                }
            }
        }

        public void saveFile(string Path)
        {
            string curLine;
            TextWriter tw = new StreamWriter(Path);

            for (int x = 0; x < 9; x++) 
            {
                curLine = "";
                for (int y = 0; y < 9; y++)
                {
                    curLine += cells[y, x].value == 0 ? "." : cells[y ,x].value.ToString();
                }

                tw.WriteLine(curLine);
            }

            tw.Close();
        }

        public void loadFile(string Path)
        {
            string curLine;
            int y = 0;
            TextReader tr = new StreamReader(Path);

            curLine = tr.ReadLine();
            while (curLine != null)
            {
                for (int x = 0; x < 9; x++)
                {
                    cells[x, y].value = curLine.Substring(x, 1) == "." ? Convert.ToByte(0) : Convert.ToByte(curLine.Substring(x, 1));
                }

                curLine = tr.ReadLine();
                y++;
            }
            tr.Close();
        }
    }
}
