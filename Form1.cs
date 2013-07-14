using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Numbrella
{
    public partial class Form1 : Form
    {
        public xBoard board;
        public xSolver solver;
        public bool newFile;
        public string filePath;
        public Thread tSolve;

        public Form1()
        {
            InitializeComponent();
            initialize();
            this.Text = "Numbrella " + Application.ProductVersion.Substring(0,3) + " - Sudoku Solver";
        }

        public void initialize()
        {
            initialize(true, true);
        }

        public void initialize(bool ResetFilePath, bool SetStatus)
        {
            newFile = true;
            board = new xBoard();
            solver = new xSolver(board);
            solver.solutionComplete += new xSolver.solverEvent(solver_solutionComplete);
            solver.puttingObviousNumbers += new xSolver.solverEvent(solver_puttingObviousNumbers);
            solver.bruteForceLeveled += new xSolver.bruteForceEvent(solver_bruteForceLeveled);

            sbMain.board = board;
            sbMain.paintBoard();
            if (ResetFilePath) filePath = "";
            if (SetStatus) setStatus("Ready...");
        }

        void solver_bruteForceLeveled(object Sender, BruteForceEventArgs e)
        {
            sbMain.paintBoard(e.board);
            setStatus("Decision tree @ level " + e.level.ToString());
        }

        void solver_puttingObviousNumbers(object Sender, EventArgs e)
        {
            setStatus("Putting obvious numbers...");
        }

        void solver_solutionComplete(object Sender, EventArgs e)
        {
            sbMain.paintBoard();
            newFile = false;
            button1.Enabled = true;
            setStatus("Solution complete...");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            sbMain.fillCells();
            if (newFile) sbMain.markNumbersAsOriginal();
            sbMain.paintBoard();
            setStatus("Solving...");
            solver.solve();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            initialize();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filePath == "") if (sfd.ShowDialog() != DialogResult.OK) return; else filePath = sfd.FileName;
            saveAs();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() != DialogResult.OK) return; else filePath = sfd.FileName;
            saveAs();
        }

        private void saveAs()
        {
            sbMain.fillCells();
            board.saveFile(filePath);
            setStatus(filePath += " saved...");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() != DialogResult.OK) return; else filePath = ofd.FileName;
            initialize(false, false);
            board.loadFile(filePath);
            sbMain.paintBoard();
            setStatus(filePath + " loaded...");
        }

        private void setStatus(string Text)
        {
            status.Text = Text;
            Application.DoEvents();
        }
    }
}