using System;
using System.Drawing;

namespace Minesweeper
{
    class Board: IDisposable
    {
        public bool gameStarted = false;
        public int mines;

        public bool won { get; private set; }
        public bool lost { get; private set; }

        public SweeperButton[,] boardButtons { get; private set; }

        private int length, height;
        private Random rand = new Random();

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (SweeperButton s in boardButtons)
                {
                    s.Dispose();
                }
            }

            disposed = true;
        }

        public Board(int length, int height, int mines, Point upperLeft, bool threeD)
        {
            this.length = length;
            this.height = height;
            this.mines = mines;

            won = false;
            lost = false;

            boardButtons = new SweeperButton[length, height];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    boardButtons[i, j] = new SweeperButton(upperLeft.X + 35 * i, upperLeft.Y + 35 * j, this, threeD);
                }
            }

            LinkButtons();
            AddMines();
        }

        public void BoardChanged()
        {
            if (!gameStarted)
            {
                gameStarted = true;
            }

            if (GameWon())
            {
                won = true;
            }
            else if (GameLost())
            {
                lost = true;
            }
        }

        private void LinkButtons()
        {
            //The following handles the 4 corner buttons
            boardButtons[0, 0].linkedButtons[0] = null;
            boardButtons[0, 0].linkedButtons[1] = null;
            boardButtons[0, 0].linkedButtons[2] = null;
            boardButtons[0, 0].linkedButtons[3] = null;
            boardButtons[0, 0].linkedButtons[4] = boardButtons[1, 0];
            boardButtons[0, 0].linkedButtons[5] = null;
            boardButtons[0, 0].linkedButtons[6] = boardButtons[0, 1];
            boardButtons[0, 0].linkedButtons[7] = boardButtons[1, 1];

            boardButtons[0, height - 1].linkedButtons[0] = null;
            boardButtons[0, height - 1].linkedButtons[1] = boardButtons[0, height - 2];
            boardButtons[0, height - 1].linkedButtons[2] = boardButtons[1, height - 2];
            boardButtons[0, height - 1].linkedButtons[3] = null;
            boardButtons[0, height - 1].linkedButtons[4] = boardButtons[1, height - 1];
            boardButtons[0, height - 1].linkedButtons[5] = null;
            boardButtons[0, height - 1].linkedButtons[6] = null;
            boardButtons[0, height - 1].linkedButtons[7] = null;

            boardButtons[length - 1, 0].linkedButtons[0] = null;
            boardButtons[length - 1, 0].linkedButtons[1] = null;
            boardButtons[length - 1, 0].linkedButtons[2] = null;
            boardButtons[length - 1, 0].linkedButtons[3] = boardButtons[length - 2, 0];
            boardButtons[length - 1, 0].linkedButtons[4] = null;
            boardButtons[length - 1, 0].linkedButtons[5] = boardButtons[length - 2, 1];
            boardButtons[length - 1, 0].linkedButtons[6] = boardButtons[length - 1, 1];
            boardButtons[length - 1, 0].linkedButtons[7] = null;

            boardButtons[length - 1, height - 1].linkedButtons[0] = boardButtons[length - 2, height - 2];
            boardButtons[length - 1, height - 1].linkedButtons[1] = boardButtons[length - 1, height - 2];
            boardButtons[length - 1, height - 1].linkedButtons[2] = null;
            boardButtons[length - 1, height - 1].linkedButtons[3] = boardButtons[length - 2, height - 1];
            boardButtons[length - 1, height - 1].linkedButtons[4] = null;
            boardButtons[length - 1, height - 1].linkedButtons[5] = null;
            boardButtons[length - 1, height - 1].linkedButtons[6] = null;
            boardButtons[length - 1, height - 1].linkedButtons[7] = null;

            //This handles the rest of the top row where j = 0
            for (int i = 1; i < length - 1; i++)
            {
                int j = 0;
                boardButtons[i, j].linkedButtons[0] = null;
                boardButtons[i, j].linkedButtons[1] = null;
                boardButtons[i, j].linkedButtons[2] = null;
                boardButtons[i, j].linkedButtons[3] = boardButtons[i - 1, j];
                boardButtons[i, j].linkedButtons[4] = boardButtons[i + 1, j];
                boardButtons[i, j].linkedButtons[5] = boardButtons[i - 1, j + 1];
                boardButtons[i, j].linkedButtons[6] = boardButtons[i, j + 1];
                boardButtons[i, j].linkedButtons[7] = boardButtons[i + 1, j + 1];
            }

            //This handles the rest of the bottom row where j = height - 1
            for (int i = 1; i < length - 1; i++)
            {
                int j = height - 1;
                boardButtons[i, j].linkedButtons[0] = boardButtons[i - 1, j - 1];
                boardButtons[i, j].linkedButtons[1] = boardButtons[i, j - 1];
                boardButtons[i, j].linkedButtons[2] = boardButtons[i + 1, j - 1];
                boardButtons[i, j].linkedButtons[3] = boardButtons[i - 1, j];
                boardButtons[i, j].linkedButtons[4] = boardButtons[i + 1, j];
                boardButtons[i, j].linkedButtons[5] = null;
                boardButtons[i, j].linkedButtons[6] = null;
                boardButtons[i, j].linkedButtons[7] = null;
            }

            //This handles the rest of the first row where i = 0
            for (int j = 1; j < height - 1; j++)
            {
                int i = 0;
                boardButtons[i, j].linkedButtons[0] = null;
                boardButtons[i, j].linkedButtons[1] = boardButtons[i, j - 1];
                boardButtons[i, j].linkedButtons[2] = boardButtons[i + 1, j - 1];
                boardButtons[i, j].linkedButtons[3] = null;
                boardButtons[i, j].linkedButtons[4] = boardButtons[i + 1, j];
                boardButtons[i, j].linkedButtons[5] = null; ;
                boardButtons[i, j].linkedButtons[6] = boardButtons[i, j + 1];
                boardButtons[i, j].linkedButtons[7] = boardButtons[i + 1, j + 1];
            }

            //This handles the rest of the last row where i = length - 1
            for (int j = 1; j < height - 1; j++)
            {
                int i = length - 1;
                boardButtons[i, j].linkedButtons[0] = boardButtons[i - 1, j - 1];
                boardButtons[i, j].linkedButtons[1] = boardButtons[i, j - 1];
                boardButtons[i, j].linkedButtons[2] = null;
                boardButtons[i, j].linkedButtons[3] = boardButtons[i - 1, j];
                boardButtons[i, j].linkedButtons[4] = null;
                boardButtons[i, j].linkedButtons[5] = boardButtons[i - 1, j + 1];
                boardButtons[i, j].linkedButtons[6] = boardButtons[i, j + 1];
                boardButtons[i, j].linkedButtons[7] = null;
            }

            //This handles the rest of the board
            for (int i = 1; i < length - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    boardButtons[i, j].linkedButtons[0] = boardButtons[i - 1, j - 1];
                    boardButtons[i, j].linkedButtons[1] = boardButtons[i, j - 1];
                    boardButtons[i, j].linkedButtons[2] = boardButtons[i + 1, j - 1];
                    boardButtons[i, j].linkedButtons[3] = boardButtons[i - 1, j];
                    boardButtons[i, j].linkedButtons[4] = boardButtons[i + 1, j];
                    boardButtons[i, j].linkedButtons[5] = boardButtons[i - 1, j + 1];
                    boardButtons[i, j].linkedButtons[6] = boardButtons[i, j + 1];
                    boardButtons[i, j].linkedButtons[7] = boardButtons[i + 1, j + 1];
                }
            }
        }

        private void AddMines()
        {
            int addedMines = 0;
            int row, col;

            while (addedMines < mines)
            {
                row = rand.Next(length);
                col = rand.Next(height);

                if (boardButtons[row, col].number != -1)
                {
                    boardButtons[row, col].AddMine();
                    addedMines++;
                }
            }
        }

        private bool GameWon()
        {
            foreach (SweeperButton s in boardButtons)
            {
                if (s.winValue == false)
                {
                    return false;
                }
            }

            return true;
        }

        private bool GameLost()
        {
            foreach (SweeperButton s in boardButtons)
            {
                if (s.BackColor == Color.Red)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
