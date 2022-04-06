using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    enum Status
    {
        Unused,
        Mine,
        Number
    }

    public partial class Form1 : Form
    {
        bool gameStarted = false;
        Field field;
        Point[] allMineCoords;

        public Form1()
        {
            InitializeComponent();
            int mineCount = 100;
            allMineCoords = new Point[mineCount];
            field = new Field(mineCount, this);
            mineCountLabel.Text = mineCount.ToString();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            int seconds = int.Parse(timerLabel.Text);
            seconds += 1;

            timerLabel.Text = seconds.ToString();
        }

        public void CellPressed(Point cellCoords, MouseButtons button)
        {
            if (!gameStarted)
            {
                StartGame(cellCoords);
                OpenCell(cellCoords);

                gameStarted = true;
                return;
            }

            switch (button)
            {
                case MouseButtons.Left:
                    OpenCell(cellCoords);
                    break;
                case MouseButtons.Right:
                    MarkCell(cellCoords);
                    break;
            }
        }

        public void OpenCell(Point cellCoords)
        {
            Status status = field.OpenCell(cellCoords);

            if (status == Status.Mine)
            {
                EndGame(false);
                return;
            }

            if (field.GameWon())
            {
                EndGame(true);
                return;
            }

            if (field.GetMineNumber(cellCoords) == 0)
            {
                OpenEmptyCells(cellCoords);
            }
        }

        private void OpenEmptyCells(Point currentCellCoords)
        {
            int minX = currentCellCoords.X - 1;
            minX = minX < 0 ? 0 : minX;
            int maxX = currentCellCoords.X + 1;
            maxX = maxX > field.GridSize.Width - 1 ? field.GridSize.Width - 1 : maxX;

            int minY = currentCellCoords.Y - 1;
            minY = minY < 0 ? 0 : minY;
            int maxY = currentCellCoords.Y + 1;
            maxY = maxY > field.GridSize.Height - 1 ? field.GridSize.Height - 1 : maxY;

            for (int i = minY; i <= maxY; i++)
            {
                for (int j = minX; j <= maxX; j++)
                {
                    var cellCoords = new Point(j, i);

                    if (field.GetIsOpen(cellCoords) || cellCoords == currentCellCoords)
                    {
                        continue;
                    }

                    field.OpenCell(cellCoords);

                    if (field.GetMineNumber(cellCoords) == 0)
                    {
                        OpenEmptyCells(cellCoords);
                    }
                }
            }
        }

        public void MarkCell(Point cellCoords)
        {
            bool marked = field.MarkUnmarkCell(cellCoords);
            int mineCount = int.Parse(mineCountLabel.Text);

            if (marked)
            {
                mineCount--;
            }
            else
            {
                mineCount++;
            }

            mineCountLabel.Text = mineCount.ToString();
        }

        public void EndGame(bool won)
        {
            timer1.Enabled = false;

            if (!won)
            {
                ShowAllMines();
            }
        }

        public void ShowAllMines()
        {
            foreach (var cellCoords in allMineCoords)
            {
                if (!field.GetIsOpen(cellCoords))
                {
                    OpenCell(cellCoords);
                }
            }
        }

        public void StartGame(Point openedCellCoords)
        {
            int mineCount = int.Parse(mineCountLabel.Text);
            Point[] minesCoords = new Point[mineCount + 9];

            AddExceptionCoords(minesCoords, openedCellCoords);

            for (int i = 9; i < mineCount; i++)
            {
                minesCoords[i] = CreateMinePoint(minesCoords, i);
                allMineCoords[i - 1] = minesCoords[i];

                field.SetStatus(minesCoords[i], Status.Mine);
            }

            for (int i = 0; i < field.GridSize.Height; i++)
            {
                for (int j = 0; j < field.GridSize.Width; j++)
                {
                    var cellCoords = new Point(j, i);

                    if (field.GetStatus(cellCoords) == Status.Unused)
                    {
                        int minesNumber = CountMinesNearCell(cellCoords);

                        field.SetStatus(cellCoords, Status.Number, minesNumber);
                    }
                }
            }
        }

        private void AddExceptionCoords(Point[] minesCoords, Point currentCellCoords)
        {
            int minX = currentCellCoords.X - 1;
            minX = minX < 0 ? 0 : minX;
            int maxX = currentCellCoords.X + 1;
            maxX = maxX > field.GridSize.Width - 1 ? field.GridSize.Width - 1 : maxX;

            int minY = currentCellCoords.Y - 1;
            minY = minY < 0 ? 0 : minY;
            int maxY = currentCellCoords.Y + 1;
            maxY = maxY > field.GridSize.Height - 1 ? field.GridSize.Height - 1 : maxY;

            int index = 0;
            for (int i = minY; i <= maxY; i++)
            {
                for (int j = minX; j <= maxX; j++)
                {
                    minesCoords[index] = new Point(j, i);

                    index++;
                }
            }
        }

        private int CountMinesNearCell(Point currentCellCoords)
        {
            int minX = currentCellCoords.X - 1;
            minX = minX < 0 ? 0 : minX;
            int maxX = currentCellCoords.X + 1;
            maxX = maxX > field.GridSize.Width - 1 ? field.GridSize.Width - 1 : maxX;

            int minY = currentCellCoords.Y - 1;
            minY = minY < 0 ? 0 : minY;
            int maxY = currentCellCoords.Y + 1;
            maxY = maxY > field.GridSize.Height - 1 ? field.GridSize.Height - 1 : maxY;

            int count = 0;
            for (int i = minY; i <= maxY; i++)
            {
                for (int j = minX; j <= maxX; j++)
                {
                    var cellCoords = new Point(j, i);

                    if (field.GetStatus(cellCoords) == Status.Mine)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public Point CreateMinePoint(Point[] mineCoords, int index)
        {
            Random random = new Random();

            int row;
            int column;
            Point newPoint;

            do
            {
                row = random.Next(0, field.GridSize.Height);
                column = random.Next(0, field.GridSize.Width);
                newPoint = new Point(column, row);

            } while (IsExist(newPoint, mineCoords, index));

            return newPoint;
        }

        public bool IsExist(Point newPoint, Point[] mineCoords, int index)
        {
            for (int i = 0; i < index; i++)
            {
                if (mineCoords[i] == newPoint)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
