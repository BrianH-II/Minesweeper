using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class GUI : Form
    {
        /*
         *  This program allows the user to play the Microsoft game Minesweeper.
         *  In addition to the traditional Minesweeper game,
         *  this program allows the user to play Minesweeper in 3D.
         *  
         *  Unfortunately, 3D minesweeper is not mathematically beatable.
         *  As such, beating 3D minesweeper is entirely based on luck.
         */ 

        public static Font times12 = new Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        public static Font times16 = new Font("Times New Roman", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        public static Font times20 = new Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

        private Label header = new Label();
        private Button[] gameChoices = new Button[3];

        private Board[] boards;
        private Label gameTimeLabel, minesLeftLabel, gameTime, minesLeft;
        private Button switchBoard = new Button();
        Timer timer = new Timer();

        private bool gameStarted = false;
        private bool gameOver = false;

        private Button reset = new Button();

        private string currentState;
        private int visibleBoard = 0;

        public GUI()
        {
            InitializeComponent();
            this.Text = "MineSweeper";
            this.Size = new Size(500, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            header.AutoSize = true;
            header.Font = times20;
            this.Controls.Add(header);

            for (int i = 0; i < gameChoices.Length; i++)
            {
                gameChoices[i] = new Button();
                gameChoices[i].Size = new Size(200, 100);
                gameChoices[i].Font = new Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                gameChoices[i].Click += new EventHandler(button_Click);

                this.Controls.Add(gameChoices[i]);
            }

            gameTimeLabel = new Label();
            gameTimeLabel.Text = "Timer";
            gameTimeLabel.AutoSize = true;
            gameTimeLabel.Font = times16;
            gameTimeLabel.ForeColor = Color.Green;

            minesLeftLabel = new Label();
            minesLeftLabel.Text = "Mines Left";
            minesLeftLabel.AutoSize = true;
            minesLeftLabel.Font = times16;
            minesLeftLabel.ForeColor = Color.Blue;

            gameTime = new Label();
            gameTime.Text = "000.0";
            gameTime.AutoSize = true;
            gameTime.Font = times16;
            gameTime.ForeColor = Color.Green;

            minesLeft = new Label();
            minesLeft.AutoSize = true;
            minesLeft.Font = times16;
            minesLeft.ForeColor = Color.Blue;

            switchBoard.Size = new Size(100, 50);
            switchBoard.Text = "Switch Board";
            switchBoard.Font = times12;
            switchBoard.Click += new EventHandler(switchBoard_Click);
            this.Controls.Add(switchBoard);
            switchBoard.Visible = false;

            timer.Interval = 10;
            timer.Tick += new EventHandler(timer_Tick);

            reset.Size = new Size(100, 50);
            reset.Text = "Play again";
            reset.Font = times12;
            reset.Click += new EventHandler(reset_Click);
            this.Controls.Add(reset);

            PositionScreenComponents("Opening");
        }

        private void PositionScreenComponents(string state)
        {
            currentState = state;
            reset.Visible = false;

            Point boardPosition;

            switch (state)
            {
                case "Opening":
                    header.Text = "Select a game below";
                    header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);

                    gameChoices[0].Text = "Play classic MineSweeper";
                    gameChoices[1].Text = "Play 3D MineSweeper";

                    gameChoices[0].Visible = true;
                    gameChoices[1].Visible = true;
                    gameChoices[2].Visible = false;

                    for (int i = 0; i < gameChoices.Length; i++)
                    {
                        gameChoices[i].Location = new Point((ClientSize.Width - gameChoices[i].Width) / 2, 100 + (gameChoices[i].Height + 10) * i);
                    }

                    break;

                case "Play classic MineSweeper":
                    header.Text = "Select a level below";
                    header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);

                    gameChoices[0].Text = "Easy\r\n(9x9)";
                    gameChoices[1].Text = "Medium\r\n(16x16)";
                    gameChoices[2].Text = "Hard\r\n(30x16)";

                    gameChoices[0].Visible = true;
                    gameChoices[1].Visible = true;
                    gameChoices[2].Visible = true;

                    break;

                case "Easy\r\n(9x9)":
                    this.Size = new Size(500, 600);
                    header.Text = "Good luck!";
                    header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);
                    minesLeft.Text = "10";

                    gameChoices[0].Visible = false;
                    gameChoices[1].Visible = false;
                    gameChoices[2].Visible = false;

                    boardPosition = new Point((ClientSize.Width - 35 * 9) / 2, 2 * (ClientSize.Height - 35 * 9) / 3);

                    boards = new Board[1];
                    boards[0] = new Board(9, 9, 10, boardPosition, false);
                    foreach (SweeperButton s in boards[0].boardButtons)
                    {
                        this.Controls.Add(s);
                    }

                    gameTimeLabel.Location = new Point((header.Location.X - gameTimeLabel.Width) / 2, 65);
                    gameTime.Location = new Point(gameTimeLabel.Location.X + 2, gameTimeLabel.Location.Y + 25);
                    this.Controls.Add(gameTimeLabel);
                    this.Controls.Add(gameTime);

                    minesLeftLabel.Location = new Point((ClientSize.Width - header.Location.X - header.Width - minesLeftLabel.Width) / 2 + header.Location.X + header.Width, 65);
                    minesLeft.Location = new Point(minesLeftLabel.Location.X + 40, minesLeftLabel.Location.Y + 25);
                    this.Controls.Add(minesLeftLabel);
                    this.Controls.Add(minesLeft);

                    timer.Start();

                    break;

                case "Medium\r\n(16x16)":
                    this.Size = new Size(700, 850);
                    header.Text = "Good luck!";
                    header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);
                    minesLeft.Text = "40";

                    gameChoices[0].Visible = false;
                    gameChoices[1].Visible = false;
                    gameChoices[2].Visible = false;

                    boardPosition = new Point((ClientSize.Width - 35 * 16) / 2, 2 * (ClientSize.Height - 35 * 16) / 3);

                    boards = new Board[1];
                    boards[0] = new Board(16, 16, 40, boardPosition, false);
                    foreach (SweeperButton s in boards[0].boardButtons)
                    {
                        this.Controls.Add(s);
                    }

                    gameTimeLabel.Location = new Point((header.Location.X - gameTimeLabel.Width) / 2, 65);
                    gameTime.Location = new Point(gameTimeLabel.Location.X + 2, gameTimeLabel.Location.Y + 25);
                    this.Controls.Add(gameTimeLabel);
                    this.Controls.Add(gameTime);

                    minesLeftLabel.Location = new Point((ClientSize.Width - header.Location.X - header.Width - minesLeftLabel.Width) / 2 + header.Location.X + header.Width, 65);
                    minesLeft.Location = new Point(minesLeftLabel.Location.X + 40, minesLeftLabel.Location.Y + 25);
                    this.Controls.Add(minesLeftLabel);
                    this.Controls.Add(minesLeft);

                    timer.Start();

                    break;

                case "Hard\r\n(30x16)":
                    this.Size = new Size(1250, 850);
                    header.Text = "Good luck!";
                    header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);
                    minesLeft.Text = "99";

                    gameChoices[0].Visible = false;
                    gameChoices[1].Visible = false;
                    gameChoices[2].Visible = false;

                    boardPosition = new Point((ClientSize.Width - 35 * 30) / 2, 2 * (ClientSize.Height - 35 * 16) / 3);

                    boards = new Board[1];
                    boards[0] = new Board(30, 16, 99, boardPosition, false);
                    foreach (SweeperButton s in boards[0].boardButtons)
                    {
                        this.Controls.Add(s);
                    }

                    gameTimeLabel.Location = new Point((header.Location.X - gameTimeLabel.Width) / 2, 65);
                    gameTime.Location = new Point(gameTimeLabel.Location.X + 2, gameTimeLabel.Location.Y + 25);
                    this.Controls.Add(gameTimeLabel);
                    this.Controls.Add(gameTime);

                    minesLeftLabel.Location = new Point((ClientSize.Width - header.Location.X - header.Width - minesLeftLabel.Width) / 2 + header.Location.X + header.Width, 65);
                    minesLeft.Location = new Point(minesLeftLabel.Location.X + 40, minesLeftLabel.Location.Y + 25);
                    this.Controls.Add(minesLeftLabel);
                    this.Controls.Add(minesLeft);

                    timer.Start();

                    break;

                case "Play 3D MineSweeper":
                    header.Text = "Select a level below";
                    header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);

                    gameChoices[0].Text = "Easy\r\n(9x9x2)";
                    gameChoices[1].Text = "Medium\r\n(16x16x3)";
                    gameChoices[2].Text = "Hard\r\n(30x16x4)";

                    gameChoices[0].Visible = true;
                    gameChoices[1].Visible = true;
                    gameChoices[2].Visible = true;

                    break;

                case "Easy\r\n(9x9x2)":
                    this.Size = new Size(500, 600);
                    header.Text = "Good luck!";
                    header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);
                    minesLeft.Text = "20";

                    gameChoices[0].Visible = false;
                    gameChoices[1].Visible = false;
                    gameChoices[2].Visible = false;

                    boardPosition = new Point((ClientSize.Width - 35 * 9) / 2, 2 * (ClientSize.Height - 35 * 9) / 3);

                    boards = new Board[2];
                    boards[0] = new Board(9, 9, 10, boardPosition, true);
                    boards[1] = new Board(9, 9, 10, boardPosition, true);
                    CombineBoardValues(9, 9);
                    LinkBoards(9, 9);

                    foreach (SweeperButton s in boards[0].boardButtons)
                    {
                        this.Controls.Add(s);
                    }
                    foreach (SweeperButton s in boards[1].boardButtons)
                    {
                        this.Controls.Add(s);
                        s.Visible = false;
                    }

                    gameTimeLabel.Location = new Point((header.Location.X - gameTimeLabel.Width) / 2, 65);
                    gameTime.Location = new Point(gameTimeLabel.Location.X + 2, gameTimeLabel.Location.Y + 25);
                    this.Controls.Add(gameTimeLabel);
                    this.Controls.Add(gameTime);

                    minesLeftLabel.Location = new Point((ClientSize.Width - header.Location.X - header.Width - minesLeftLabel.Width) / 2 + header.Location.X + header.Width, 65);
                    minesLeft.Location = new Point(minesLeftLabel.Location.X + 40, minesLeftLabel.Location.Y + 25);
                    this.Controls.Add(minesLeftLabel);
                    this.Controls.Add(minesLeft);

                    switchBoard.Location = new Point((ClientSize.Width - switchBoard.Width) / 2, ClientSize.Height - switchBoard.Height);
                    switchBoard.Visible = true;

                    timer.Start();

                    break;

                case "Medium\r\n(16x16x3)":
                    this.Size = new Size(700, 850);
                    header.Text = "Good luck!";
                    header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);
                    minesLeft.Text = "120";

                    gameChoices[0].Visible = false;
                    gameChoices[1].Visible = false;
                    gameChoices[2].Visible = false;

                    boardPosition = new Point((ClientSize.Width - 35 * 16) / 2, 2 * (ClientSize.Height - 35 * 16) / 3);

                    boards = new Board[3];
                    boards[0] = new Board(16, 16, 40, boardPosition, true);
                    boards[1] = new Board(16, 16, 40, boardPosition, true);
                    boards[2] = new Board(16, 16, 40, boardPosition, true);
                    CombineBoardValues(16, 16);
                    LinkBoards(16, 16);

                    foreach (SweeperButton s in boards[0].boardButtons)
                    {
                        this.Controls.Add(s);
                    }
                    foreach (SweeperButton s in boards[1].boardButtons)
                    {
                        this.Controls.Add(s);
                        s.Visible = false;
                    }
                    foreach (SweeperButton s in boards[2].boardButtons)
                    {
                        this.Controls.Add(s);
                        s.Visible = false;
                    }

                    gameTimeLabel.Location = new Point((header.Location.X - gameTimeLabel.Width) / 2, 65);
                    gameTime.Location = new Point(gameTimeLabel.Location.X + 2, gameTimeLabel.Location.Y + 25);
                    this.Controls.Add(gameTimeLabel);
                    this.Controls.Add(gameTime);

                    minesLeftLabel.Location = new Point((ClientSize.Width - header.Location.X - header.Width - minesLeftLabel.Width) / 2 + header.Location.X + header.Width, 65);
                    minesLeft.Location = new Point(minesLeftLabel.Location.X + 40, minesLeftLabel.Location.Y + 25);
                    this.Controls.Add(minesLeftLabel);
                    this.Controls.Add(minesLeft);

                    switchBoard.Location = new Point((ClientSize.Width - switchBoard.Width) / 2, ClientSize.Height - switchBoard.Height);
                    switchBoard.Visible = true;

                    timer.Start();

                    break;

                case "Hard\r\n(30x16x4)":
                    this.Size = new Size(1250, 850);
                    header.Text = "Good luck!";
                    header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);
                    minesLeft.Text = "396";

                    gameChoices[0].Visible = false;
                    gameChoices[1].Visible = false;
                    gameChoices[2].Visible = false;

                    boardPosition = new Point((ClientSize.Width - 35 * 30) / 2, 2 * (ClientSize.Height - 35 * 16) / 3);

                    boards = new Board[4];
                    boards[0] = new Board(30, 16, 99, boardPosition, true);
                    boards[1] = new Board(30, 16, 99, boardPosition, true);
                    boards[2] = new Board(30, 16, 99, boardPosition, true);
                    boards[3] = new Board(30, 16, 99, boardPosition, true);
                    CombineBoardValues(30, 16);
                    LinkBoards(30, 16);

                    foreach (SweeperButton s in boards[0].boardButtons)
                    {
                        this.Controls.Add(s);
                    }
                    foreach (SweeperButton s in boards[1].boardButtons)
                    {
                        this.Controls.Add(s);
                        s.Visible = false;
                    }
                    foreach (SweeperButton s in boards[2].boardButtons)
                    {
                        this.Controls.Add(s);
                        s.Visible = false;
                    }
                    foreach (SweeperButton s in boards[3].boardButtons)
                    {
                        this.Controls.Add(s);
                        s.Visible = false;
                    }

                    gameTimeLabel.Location = new Point((header.Location.X - gameTimeLabel.Width) / 2, 65);
                    gameTime.Location = new Point(gameTimeLabel.Location.X + 2, gameTimeLabel.Location.Y + 25);
                    this.Controls.Add(gameTimeLabel);
                    this.Controls.Add(gameTime);

                    minesLeftLabel.Location = new Point((ClientSize.Width - header.Location.X - header.Width - minesLeftLabel.Width) / 2 + header.Location.X + header.Width, 65);
                    minesLeft.Location = new Point(minesLeftLabel.Location.X + 40, minesLeftLabel.Location.Y + 25);
                    this.Controls.Add(minesLeftLabel);
                    this.Controls.Add(minesLeft);

                    switchBoard.Location = new Point((ClientSize.Width - switchBoard.Width) / 2, ClientSize.Height - switchBoard.Height);
                    switchBoard.Visible = true;

                    timer.Start();

                    break;
            }

            reset.Location = new Point((ClientSize.Width - reset.Width) / 2, 75);
        }

        private void CombineBoardValues(int length, int height)
        {
            List<int[,]> newBoardValues = new List<int[,]>();
            for (int count = 0; count < boards.Length; count++)
            {
                int[,] values = new int[length, height];
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int number = boards[count].boardButtons[i, j].number;
                        if (number != -1)
                        {
                            if (count > 0)
                            {
                                if (boards[count - 1].boardButtons[i, j].number != -1)
                                {
                                    number = number + boards[count - 1].boardButtons[i, j].number;
                                }
                                else
                                {
                                    number++;
                                }
                            }

                            if (count < boards.Length - 1)
                            {
                                if (boards[count + 1].boardButtons[i, j].number != -1)
                                {
                                    number = number + boards[count + 1].boardButtons[i, j].number;
                                }
                                else
                                {
                                    number++;
                                }
                            }
                        }

                        values[i, j] = number;
                    }
                }

                newBoardValues.Add(values);
            }

            for (int secondCount = 0; secondCount < boards.Length; secondCount++)
            {
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        boards[secondCount].boardButtons[i, j].changeNumber(newBoardValues[secondCount][i, j]);
                    }
                }
            }
        }

        private void LinkBoards(int length, int height)
        {
            for (int count = 0; count < boards.Length; count++)
            {
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (count > 0)
                        {
                            boards[count].boardButtons[i, j].lower3DLinkedButton = boards[count - 1].boardButtons[i, j];
                        }

                        if (count < boards.Length - 1)
                        {
                            boards[count].boardButtons[i, j].upper3DLinkedButton = boards[count + 1].boardButtons[i, j];
                        }
                    }
                }
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            PositionScreenComponents(((Button)sender).Text);
        }

        private void switchBoard_Click(object sender, EventArgs e)
        {
            foreach (SweeperButton s in boards[visibleBoard].boardButtons)
            {
                s.Visible = false;
            }

            if (visibleBoard + 1 == boards.Length)
            {
                visibleBoard = 0;
            }
            else
            {
                visibleBoard++;
            }

            foreach (SweeperButton s in boards[visibleBoard].boardButtons)
            {
                s.Visible = true;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!gameStarted)
            {
                foreach (Board b in boards)
                {
                    if (b.gameStarted)
                    {
                        gameStarted = true;

                        timer.Interval = 100;
                        timer.Stop();
                        timer.Start();
                    }
                }
            }
            else if (!gameOver)
            {
                //Since the game has started and is not yet over, the program checks the state of the game
                //The program checks each board individually for that board's won/lost value
                int[] boardComplete = new int[boards.Length];
                for (int i = 0; i < boardComplete.Length; i++)
                {
                    if (boards[i].won)
                    {
                        boardComplete[i] = 1;
                    }
                    else if (boards[i].lost)
                    {
                        //If one board is lost, then the player clicked a mine and lost the entire game
                        gameOver = true;

                        header.Text = "You clicked a mine!";
                        header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);

                        reset.Visible = true;

                        return;
                    }
                    else
                    {
                        boardComplete[i] = 0;
                    }
                }

                bool gameWon = true;
                for (int i = 0; i < boardComplete.Length; i++)
                {
                    if (boardComplete[i] != 1)
                    {
                        gameWon = false;
                    }
                }

                if (gameWon)
                {
                    //If each board is won, then the player found all the mines and won the entire game
                    gameOver = true;

                    header.Text = "Nice job! You won!";
                    header.Location = new Point((ClientSize.Width - header.Width) / 2, 25);

                    return;
                }

                //Due to the return statements, this code will only activate if the game has started, but is neither won nor lost yet
                //This code updates the time displayed, as well as the mines remaining
                double time = Convert.ToDouble(gameTime.Text);
                time = time + .1;

                string outTime = time.ToString();
                if (time < 100)
                {
                    if (time < 10)
                    {
                        outTime = "00" + outTime;
                    }
                    else
                    {
                        outTime = "0" + outTime;
                    }
                }
                if (!outTime.Contains("."))
                {
                    outTime = outTime + ".0";
                }

                gameTime.Text = outTime;

                int minesLeftInt = 0;
                foreach (Board b in boards)
                {
                    minesLeftInt = minesLeftInt + b.mines;
                }
                minesLeft.Text = minesLeftInt.ToString();
            }
        }

        private void reset_Click(object sender, EventArgs e)
        {
            foreach (Board b in boards)
            {
                b.Dispose();
            }
            gameStarted = false;
            gameOver = false;
            gameTime.Text = "000.0";
            PositionScreenComponents(currentState);
        }
    }
}
