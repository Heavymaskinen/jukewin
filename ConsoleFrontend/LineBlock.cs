using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ConsoleFrontend
{
    public class LineBlock
    {
        private IList<string> lines;
        private Background background;
        private ConsoleColor backgroundColor;
        private ConsoleColor foregroundColor;
        private int width;

        public LineBlock()
        {
            width                   = 20;
            MinHeight               = 10;
            lines                   = new List<string>();
            SelectedIndex           = -1;
            SelectedBackgroundColor = ConsoleColor.DarkGreen;

            background = new Background
            {
                Width           = width, MinHeight = this.MinHeight, BackgroundColor = ConsoleColor.Gray,
                ForegroundColor = ConsoleColor.Black
            };
        }

        public int Top { get; set; }
        public int Left { get; set; }

        public int Width
        {
            get => width;
            set { width = value;
                Background.Width = value;
            }
        }

        public int MinHeight { get; set; }

        public int SelectedIndex { get; set; }

        public Background Background => background;

        public ConsoleColor BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                Background.BackgroundColor = value;
            }
        }

        public ConsoleColor ForegroundColor
        {
            get => foregroundColor;
            set => foregroundColor = value;
        }

        public ConsoleColor SelectedBackgroundColor { get; set; }

        public void AddLine(string line)
        {
            lines.Add(line);
        }

        public void Draw()
        {
            DrawBackground();
            DrawLines();
            Console.ResetColor();
        }

        public void SmallRedraw()
        {
            DrawLines();
            Console.ResetColor();
        }

        private void DrawBackground()
        {
            SetStandardColors();
            background.Top  = Top;
            background.Left = Left;
            background.AdjustHeight(MinHeight);
            background.Draw();
        }

        private void SetStandardColors()
        {
            Console.ForegroundColor = ForegroundColor;
            Console.BackgroundColor = BackgroundColor;
        }

        private void DrawLines()
        {
            var offset = 0;
            foreach (var line in lines)
            {
                if (offset == SelectedIndex)
                {
                    SetSelectedColors();
                }
                else
                {
                    SetStandardColors();
                }

                Console.SetCursorPosition(Left, Top+offset);
                Console.Write(line);
                offset++;
            }
        }

        private void SetSelectedColors()
        {
            Console.ForegroundColor = ForegroundColor;
            Console.BackgroundColor = SelectedBackgroundColor;
        }

        public void Clear()
        {
            lines.Clear();
        }
    }
}