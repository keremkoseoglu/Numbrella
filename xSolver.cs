using System;
using System.Collections.Generic;
using System.Text;

namespace Numbrella
{
    public class xSolver
    {
        public xBoard b;
        public delegate void solverEvent(object Sender, EventArgs e);
        public delegate void bruteForceEvent(object Sender, BruteForceEventArgs e);
        public event solverEvent solutionComplete;
        public event solverEvent puttingObviousNumbers;
        public event bruteForceEvent bruteForceLeveled;
        private int bruteForceLevel;

        public xSolver(xBoard B)
        {
            b = B;
        }

        public void solve()
        {
            fillObviousNumbers(ref b, true);

            bruteForce(ref b); 

            solutionComplete(this, new EventArgs());
        }

        private void fillObviousNumbers(ref xBoard Board, bool RaiseEvent)
        {
            bool cont = true;

            if (RaiseEvent) puttingObviousNumbers(this, new EventArgs());

            if (!validate(ref Board)) return;

            while (cont)
            {
                xBoard former = new xBoard();
                for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) former.cells[x, y].value = Board.cells[x, y].value;

                fill8Boxes(ref Board);
                fill8Columns(ref Board);
                fill8Rows(ref Board);
                plusScan(ref Board);

                cont = false;
                for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (former.cells[x, y].value != Board.cells[x, y].value) cont = true;
            }
        }

        /// <summary>
        /// Recursive bir þekilde araya sayýlar atmayý dener
        /// </summary> 
        public void bruteForce(ref xBoard Board)
        {
            xBoard lBoard = new xBoard();
            xBoard lBoardBefore = new xBoard();
            Board.copyTo(ref lBoard);

            bruteForceLevel++;
            bruteForceLeveled(this, new BruteForceEventArgs(bruteForceLevel, lBoard));

            fillObviousNumbers(ref lBoard, false);
            for (byte x = 0; x < 9; x++) for (byte y = 0; y < 9; y++)
                {
                    if (lBoard.cells[x, y].value == 0)
                    {
                        for (byte a = 1; a <= 9; a++)
                        {
                            lBoardBefore = new xBoard();
                            lBoard.copyTo(ref lBoardBefore);
                            lBoard.cells[x, y].value = a;
                            lBoard.cells[x, y].source = xCell.SOURCE.BRUTE;

                            if (validate(ref lBoard))
                            {
                                lBoard.cells[x, y].source = xCell.SOURCE.COMPUTER;
                                bruteForce(ref lBoard);
                            }
                            else
                            {
                                lBoardBefore.copyTo(ref lBoard);
                            }
                        }
                    }
                }

            if (validate(ref lBoard))
            {
                fillObviousNumbers(ref lBoard, false);
                lBoard.copyTo(ref Board);
            }

            bruteForceLevel--;
        }

        /// <summary>
        /// Kutulardaki eksik sayýlarý tespit eder ve yukarýdan aþaðýya, saðdan sola bakýp dolabilecek sayýlarý doldurur
        /// </summary> 
        public void plusScan(ref xBoard Board)
        {
            byte[] missing;
            string[,] curBox;
            string[,] curBoxInitial;

            // Her bir kutu için...
            for (byte bx = 0; bx < 3; bx++)
            {
                for (byte by = 0; by < 3; by++)
                {
                    // Initialize
                    missing = new byte[10];
                    for (byte n = 1; n <= 9; n++) missing[n] = n;
                    curBox = new string[3, 3];
                    curBoxInitial = new string[3, 3];
                    for (byte x = 0; x < 3; x++) for (byte y = 0; y < 3; y++) if (Board.cells[(bx * 3) + x, (by * 3) + y].value != 0) curBoxInitial[x, y] = "."; else curBoxInitial[x, y] = "";

                    // Kutudaki eksik sayýlarý tespit edelim
                    for (byte x = 0; x < 3; x++) for (byte y = 0; y < 3; y++)
                        {
                            byte no = Board.cells[(bx * 3) + x, (by * 3) + y].value;
                            if (no != 0) missing[no] = 0;
                        }

                    // Her bir eksik sayý için...
                    for (byte n = 1; n <= 9; n++)
                    {
                        byte curno = missing[n];
                        for (byte x = 0; x < 3; x++) for (byte y = 0; y < 3; y++) curBox[x, y] = curBoxInitial[x, y];
                        if (curno != 0)
                        {
                            // Sütunu sabit tutup, sol ve saðýndaki bütün satýrlarý tarayalým
                            for (byte bx2 = 0; bx2 < 3; bx2++)
                            {
                                if (bx2 != bx)
                                {
                                    for (byte x = 0; x < 3; x++) for (byte y = 0; y < 3; y++)
                                        {
                                            if (Board.cells[(bx2 * 3) + x, (by * 3) + y].value == curno)
                                            {
                                                for (byte a = 0; a < 3; a++) curBox[a, y] = ".";
                                            }
                                        }
                                }
                            }

                            // Satýrý sabit tutup, üst ve altýndaki bütün satýrlarý tarayalým
                            for (byte by2 = 0; by2 < 3; by2++)
                            {
                                if (by2 != by)
                                {
                                    for (byte x = 0; x < 3; x++) for (byte y = 0; y < 3; y++)
                                        {
                                            if (Board.cells[(bx * 3) + x, (by2 * 3) + y].value == curno)
                                            {
                                                for (byte a = 0; a < 3; a++) curBox[x, a] = ".";
                                            }
                                        }
                                }
                            }

                            // Hem yukarýdan aþaðýya, hem soldan saða tarama yapalým
                            for (byte bx2 = 0; bx2 < 3; bx2++) for (byte by2 = 0; by2 < 3; by2++)
                                {
                                    if ((bx2 != bx && by2 == by) || (bx2 == bx && by2 != by))
                                    {
                                        for (byte x = 0; x < 3; x++) for (byte y = 0; y < 3; y++)
                                            {
                                                if (Board.cells[(bx2 * 3) + x, (by * 3) + y].value == curno)
                                                {
                                                    if (bx2 == bx)
                                                    {
                                                        for (byte a = 0; a < 3; a++) curBox[x, a] = ".";
                                                    }
                                                    else
                                                    {
                                                        for (byte a = 0; a < 3; a++) curBox[a, y] = ".";
                                                    }
                                                }
                                            }
                                    }
                                }

                            // Eðer geriye tek bir boþluk kaldýysa, söz konusu sayý ancak oraya gelebilir
                            byte c = 0;
                            for (byte x = 0; x < 3; x++) for (byte y = 0; y < 3; y++) if (curBox[x, y] == ".") c++;
                            if (c == 8) for (byte x = 0; x < 3; x++) for (byte y = 0; y < 3; y++) if (curBox[x, y] != ".")
                                        {
                                            Board.cells[(bx * 3) + x, (by * 3) + y].value = curno;
                                            Board.cells[(bx * 3) + x, (by * 3) + y].source = xCell.SOURCE.COMPUTER;
                                        }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tahtayý kontrol eder ve hatalýlarý iþaretler
        /// </summary> 
        public bool validate(ref xBoard Board)
        {
            for (byte x = 0; x < 9; x++) for (byte y = 0; y < 9; y++) Board.cells[x, y].error = false;
            bool b1 = validateBoxes(ref Board);
            bool b2 = validateRows(ref Board);
            bool b3 = validateColumns(ref Board);
            return (b1 && b2 && b3);
        }

        /// <summary>
        /// Sütun içeriklerini valide eder
        /// </summary>        
        public bool validateColumns(ref xBoard Board)
        {
            byte[] no;
            bool ret = true;

            for (byte y = 0; y < 9; y++)
            {
                no = new byte[9];

                for (byte x = 0; x < 9; x++)
                {
                    byte v = Board.cells[x, y].value;
                    if (v != 0) no[v - 1]++;
                }

                for (byte n = 0; n < 9; n++)
                {
                    if (no[n] > 1)
                    {
                        ret = false;
                        for (byte x = 0; x < 9; x++) if (Board.cells[x, y].value == n + 1) Board.cells[x, y].error = true;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Satýr içeriklerini valide eder
        /// </summary>        
        public bool validateRows(ref xBoard Board)
        {
            byte[] no;
            bool ret = true;

            for (byte x = 0; x < 9; x++)
            {
                no = new byte[9];
                for (byte y = 0; y < 9; y++)
                {
                    byte v = Board.cells[x, y].value;
                    if (v != 0) no[v - 1]++;
                }

                for (byte n = 0; n < 9; n++)
                {
                    if (no[n] > 1)
                    {
                        ret = false;
                        for (byte y = 0; y < 9; y++) if (Board.cells[x, y].value == n + 1) Board.cells[x, y].error = true;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Kutu içeriklerini valide eder
        /// </summary>        
        public bool validateBoxes(ref xBoard Board)
        {
            byte[] no;
            bool ret = true;

            for (byte bx = 0; bx < 3; bx++) for (byte by = 0; by < 3; by++)
                {
                    no = new byte[9];

                    for (byte x = 0; x < 3; x++) for (byte y = 0; y < 3; y++)
                        {
                            byte v = Board.cells[(bx * 3) + x, (by * 3) + y].value;
                            if (v != 0) no[v - 1]++;
                        }

                    for (byte n = 0; n < 9; n++)
                    {
                        if (no[n] > 1)
                        {
                            ret = false;
                            for (byte x = 0; x < 3; x++) for (byte y = 0; y < 3; y++) if (Board.cells[(bx * 3) + x, (by * 3) + y].value == n + 1) Board.cells[(bx * 3) + x, (by * 3) + y].error = true;
                        }
                    }
                }

            return ret;
        }

        /// <summary>
        /// 8 sayý içeren satýrlara 9. sayýyý yazar
        /// </summary>    
        public void fill8Rows(ref xBoard Board)
        {
            bool[] no;
            byte trueCount;
            byte falseX, falseY;

            for (byte y = 0; y < 9; y++)
            {
                no = new bool[9];
                trueCount = falseX = falseY = 0;

                for (byte x = 0; x < 9; x++)
                {
                    byte v = Board.cells[x, y].value;
                    if (v != 0)
                    {
                        no[v - 1] = true;
                        trueCount++;
                    }
                    else
                    {
                        falseX = x;
                        falseY = y;
                    }
                }

                if (trueCount == 8)
                {
                    for (byte n = 0; n < 9; n++)
                    {
                        if (!no[n])
                        {
                            Board.cells[falseX, falseY].value = Convert.ToByte(n + 1);
                            Board.cells[falseX, falseY].source = xCell.SOURCE.COMPUTER;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 8 sayý içeren sütunlara 9. sayýyý yazar
        /// </summary>    
        public void fill8Columns(ref xBoard Board)
        {
            bool[] no;
            byte trueCount;
            byte falseX, falseY;

            for (byte x = 0; x < 9; x++)
            {
                no = new bool[9];
                trueCount = falseX = falseY = 0;

                for (byte y = 0; y < 9; y++)
                {
                    byte v = Board.cells[x, y].value;
                    if (v != 0)
                    {
                        no[v - 1] = true;
                        trueCount++;
                    }
                    else
                    {
                        falseX = x;
                        falseY = y;
                    }
                }

                if (trueCount == 8)
                {
                    for (byte n = 0; n < 9; n++)
                    {
                        if (!no[n])
                        {
                            Board.cells[falseX, falseY].value = Convert.ToByte(n + 1);
                            Board.cells[falseX, falseY].source = xCell.SOURCE.COMPUTER;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 8 sayý içeren kutulara 9. sayýyý yazar
        /// </summary>        
        public void fill8Boxes(ref xBoard Board)
        {
            bool[] no;
            byte trueCount;
            byte falseX, falseY;

            for (byte bx = 0; bx < 3; bx++) for (byte by = 0; by < 3; by++)
                {
                    no = new bool[9];
                    trueCount = falseX = falseY = 0;

                    for (byte x = 0; x < 3; x++) for (byte y = 0; y < 3; y++)
                        {
                            byte v = Board.cells[(bx * 3) + x, (by * 3) + y].value;
                            if (v != 0)
                            {
                                no[v - 1] = true;
                                trueCount++;
                            }
                            else
                            {
                                falseX = x;
                                falseY = y;
                            }
                        }

                    if (trueCount == 8)
                    {
                        for (byte n = 0; n < 9; n++)
                        {
                            if (!no[n])
                            {
                                Board.cells[(bx * 3) + falseX, (by * 3) + falseY].value = Convert.ToByte(n + 1);
                                Board.cells[(bx * 3) + falseX, (by * 3) + falseY].source = xCell.SOURCE.COMPUTER;
                                break;
                            }
                        }
                    }
                }
        }
    }
}
