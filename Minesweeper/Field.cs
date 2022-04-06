using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    class Field
    {
        private readonly Form1 form;
        private readonly Panel fieldPanel;
        private readonly Cell[,] field;
        private readonly Size fieldSize;
        private readonly Size gridSize = new Size(17, 20);
        private readonly Size cellSize = new Size(30, 30);
        private readonly int cellCount;
        private readonly int mineCount;
        private int openedCellsCount = 0;

        public Size GridSize
        {
            get { return gridSize; }
        }

        public bool MarkUnmarkCell(Point cellCoords)
        {
            Cell cell = field[cellCoords.Y, cellCoords.X];

            bool marked = cell.Marked;
            if (marked)
            {
                cell.UnmarkCell();
            }
            else
            {
                cell.MarkCell();
            }

            return !marked;
        }

        public bool GameWon()
        {
            return openedCellsCount == cellCount - mineCount; 
        }

        public void SetStatus(Point cellCoords, Status cellStatus, int mineNumber = -1)
        {
            field[cellCoords.Y, cellCoords.X].SetStatus(cellStatus, mineNumber);
        }

        public bool GetIsOpen(Point cellCoords)
        {
            return field[cellCoords.Y, cellCoords.X].IsOpen;
        }

        public Status GetStatus(Point cellCoords)
        {
            return field[cellCoords.Y, cellCoords.X].CellStatus;
        }

        public int GetMineNumber(Point cellCoords)
        {
            return field[cellCoords.Y, cellCoords.X].MineNumber;
        }

        public Status OpenCell(Point cellCoords)
        {
            if (!GetIsOpen(cellCoords))
            {
                field[cellCoords.Y, cellCoords.X].OpenCell();
                openedCellsCount++;
            }

            return GetStatus(cellCoords);
        }

        public Field(int mineCount, Form1 form)
        {
            this.form = form;
            fieldPanel = new Panel();

            field = new Cell[gridSize.Height, gridSize.Width];

            int height = gridSize.Height * cellSize.Height;
            int width = gridSize.Width * cellSize.Width;
            fieldSize = new Size(width, height);

            fieldPanel.Size = fieldSize;
            form.Controls.Add(fieldPanel);

            cellCount = width * height;
            this.mineCount = mineCount;

            InitializeField();
            fieldPanel.Location = new Point(35, 100);
        }

        private void InitializeField()
        {
            int width = fieldSize.Width / cellSize.Width;
            int height = fieldSize.Height / cellSize.Height;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Point location = new Point(cellSize.Height * j, cellSize.Width * i);
                    Cell cell = new Cell(location, cellSize, form);
                    field[i, j] = cell;
                    fieldPanel.Controls.Add(cell.CellBody);
                }
            }
        }
    }

    class Cell
    {
        private Button cellBody;
        private Color color = Color.Green;
        private Form1 form;
        private Point coords;
        private Status cellStatus;
        private int mineNumber;
        private bool marked = false;
        private bool isOpen = false;

        public bool IsOpen
        {
            get { return isOpen; }
        }

        public bool Marked
        {
            get { return marked; }
        }

        public Status CellStatus
        {
            get { return cellStatus; }
        }

        public int MineNumber
        {
            get { return mineNumber; }
        }

        public void SetStatus(Status cellStatus, int mineNumber = -1)
        {
            this.cellStatus = cellStatus;
            this.mineNumber = mineNumber;
        }

        public Button CellBody
        {
            get { return cellBody; }
        }

        public Cell(Point location, Size size, Form1 form)
        {
            this.form = form;

            coords = new Point(location.X / size.Width, location.Y / size.Width);

            cellBody = new Button();
            cellBody.Location = location;
            cellBody.FlatAppearance.BorderSize = 0;
            cellBody.Size = size;
            cellBody.BackColor = color;
            cellBody.FlatStyle = FlatStyle.System;

            cellBody.MouseClick += Cell_OnClick;

            cellStatus = Status.Unused;

            form.Controls.Add(cellBody);
        }

        private void Cell_OnClick(object sender, MouseEventArgs e)
        {
            var button = e.Button;

            form.CellPressed(coords, button);
        }

        public void OpenCell()
        {
            cellBody.Enabled = false;

            string text = mineNumber > 0 ? mineNumber.ToString() : mineNumber == 0 ? "" : "B";

            cellBody.Text = text;


            isOpen = true;
        }

        public void MarkCell()
        {
            marked = true;

            cellBody.Text = "M";
        }

        public void UnmarkCell()
        {
            marked = false;

            cellBody.Text = "";
        }
    }
}