using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    class SweeperButton : Button
    {
        private bool threeD;

        public SweeperButton[] linkedButtons = new SweeperButton[8];
        public SweeperButton lower3DLinkedButton;
        public SweeperButton upper3DLinkedButton;

        public bool winValue { get; private set; }
        public int number { get; private set; }
        public void changeNumber(int newValue)
        {
            if(threeD)
            {
                number = newValue;
            }
        }

        private Board board;
        private MouseEventArgs args;

        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (threeD)
            {
                Rectangle rect = ClientRectangle;
                using (StringFormat sf = new StringFormat())
                {
                    using (Brush brush = new SolidBrush(ForeColor))
                    {
                        Font newFont = new Font(Font.FontFamily, Font.Size - 6);

                        if (upper3DLinkedButton != null)
                        {
                            //Upper-right text
                            sf.Alignment = StringAlignment.Far;
                            sf.LineAlignment = StringAlignment.Near;

                            if (upper3DLinkedButton.BackColor == Color.Green)
                            {
                                using (Brush greenBrush = new SolidBrush(Color.Green))
                                {
                                    e.Graphics.DrawString("M", newFont, greenBrush, rect, sf);
                                }
                            }
                            else
                            {
                                e.Graphics.DrawString(upper3DLinkedButton.Text, newFont, brush, rect, sf);
                            }
                        }

                        if (lower3DLinkedButton != null)
                        {
                            //Lower-left text
                            sf.Alignment = StringAlignment.Near;
                            sf.LineAlignment = StringAlignment.Far;

                            if (lower3DLinkedButton.BackColor == Color.Green)
                            {
                                using (Brush greenBrush = new SolidBrush(Color.Green))
                                {
                                    e.Graphics.DrawString("M", newFont, greenBrush, rect, sf);
                                }
                            }
                            else
                            {
                                e.Graphics.DrawString(lower3DLinkedButton.Text, newFont, brush, rect, sf);
                            }
                        }
                    }
                }
            }
        }

        public SweeperButton(int xLocation, int yLocation, Board board, bool threeD)
        {
            this.threeD = threeD;

            this.Size = new Size(35, 35);
            this.Location = new Point(xLocation, yLocation);
            this.SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
            this.Font = GUI.times16;

            this.MouseUp += new MouseEventHandler(boardButton_MouseUp);
            this.DoubleClick += new EventHandler(boardButton_DoubleClick);

            this.board = board;
        }

        public void AddMine()
        {
            number = -1;
            winValue = true;

            foreach (SweeperButton s in linkedButtons)
            {
                if (s != null)
                {
                    s.LinkMine();
                }
            }
        }

        private void LinkMine()
        {
            if (number != -1)
            {
                number++;
            }
        }

        private void boardButton_MouseUp(object sender, MouseEventArgs e)
        {
            args = e;

            if (!board.won && !board.lost)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (this.BackColor == SystemColors.Control && this.Text == "")
                    {
                        if (number == 0)
                        {
                            this.Text = number.ToString();
                            winValue = true;

                            foreach (SweeperButton s in linkedButtons)
                            {
                                if (!ReferenceEquals(s, null))
                                {
                                    try
                                    {
                                        s.boardButton_MouseUp(sender, e);
                                    }
                                    catch { }
                                }
                            }
                        }
                        else if (number == -1)
                        {
                            this.BackColor = Color.Red;
                            winValue = false;
                        }
                        else
                        {
                            this.Text = number.ToString();
                            winValue = true;
                        }
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (this.Text == "")
                    {
                        if (this.BackColor == SystemColors.Control)
                        {
                            this.BackColor = Color.Green;
                            board.mines--;
                        }
                        else if (this.BackColor == Color.Green)
                        {
                            this.BackColor = SystemColors.Control;
                            this.UseVisualStyleBackColor = true;
                            board.mines++;
                        }
                    }
                }

                board.BoardChanged();
            }
        }

        private void boardButton_DoubleClick(object sender, EventArgs e)
        {
            if (args.Button == MouseButtons.Left)
            {
                int count = 0;
                foreach (SweeperButton s in linkedButtons)
                {
                    if (!ReferenceEquals(s, null))
                    {
                        if(s.BackColor == Color.Green)
                        {
                            count++;
                        }
                    }
                }

                if (count == number)
                {
                    foreach (SweeperButton s in linkedButtons)
                    {
                        if (!ReferenceEquals(s, null))
                        {
                            s.boardButton_MouseUp(sender, args);
                        }
                    }
                }
            }
        }
    }
}
