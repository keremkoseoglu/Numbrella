using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Numbrella
{
    public partial class SudokuBoard : UserControl
    {
        public xBoard board;
        public TextBox[,] box;

        public SudokuBoard()
        {
            InitializeComponent();
        }

        private void SudokuBoard_Load(object sender, EventArgs e)
        {
            box = new TextBox[9, 9];

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    box[x, y] = new TextBox();
                    box[x, y].Text = "";
                    box[x, y].Left = 12 + (x * 33);
                    box[x, y].Top = 10 + (y * 33);
                    box[x, y].Width = 15;
                    box[x, y].Font = new Font("Comic Sans", 12);
                    box[x, y].BorderStyle = BorderStyle.None;
                    box[x, y].MaxLength = 1;
                    this.Controls.Add(box[x, y]);
                }
            }
            this.Controls.SetChildIndex(pictureBox1, 99);
        }

        public void paintBoard(xBoard Board)
        {
            for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++)
                {
                    try
                    {
                        box[x, y].Text = (Board.cells[x, y].value != 0 ? Board.cells[x, y].value.ToString() : "");
                        if (Board.cells[x, y].error)
                        {
                            box[x, y].ForeColor = Color.Red;
                        }
                        else
                        {
                            switch (Board.cells[x, y].source)
                            {
                                case xCell.SOURCE.COMPUTER:
                                    box[x, y].ForeColor = Color.HotPink;
                                    break;
                                case xCell.SOURCE.HUMAN:
                                    box[x, y].ForeColor = Color.Blue;
                                    break;
                                case xCell.SOURCE.BRUTE:
                                    box[x, y].ForeColor = Color.Gray;
                                    break;
                                default:
                                    box[x, y].ForeColor = Color.Black;
                                    break;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
        }

        public void paintBoard()
        {
            paintBoard(board);
        }

        public void fillCells()
        {
            for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++)
                {
                    try
                    {
                        board.cells[x, y].value = Convert.ToByte(box[x, y].Text);
                    }
                    catch
                    {
                        board.cells[x, y].value = 0;
                    }

                    if (board.cells[x, y].source == xCell.SOURCE.UNKNOWN) board.cells[x, y].source = xCell.SOURCE.HUMAN;
                }
        }

        public void markNumbersAsOriginal()
        {
            for (int x = 0; x < 9; x++) for (int y = 0; y < 9; y++) if (board.cells[x,y].value != 0) board.cells[x, y].source = xCell.SOURCE.QUESTION;
        }
    }
}
