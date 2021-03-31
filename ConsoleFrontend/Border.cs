using System;
using System.Text;

namespace ConsoleFrontend
{
    public class Border : IDrawable
    {
        private char horzFill;
        private char vertFill;

        public Border(char horzFill, char vertFill, int width, int height)
        {
            this.horzFill = horzFill;
            this.vertFill = vertFill;
            Width         = width;
            MinHeight     = height;
        }

        public int MinHeight { get; set; }

        public int Width { get; set; }

        public int Top { get; set; }
        public int Left { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public void Draw()
        {
            Console.ForegroundColor = ForegroundColor;
            Console.BackgroundColor = BackgroundColor;
            var fullLineBuilder = new StringBuilder();
            for (var i = 0; i < Width; i++)
            {
                fullLineBuilder.Append(horzFill);
            }

            var fullLine = fullLineBuilder.ToString();
            Console.SetCursorPosition(Left, Top);
            Console.Write(fullLine);
            Console.SetCursorPosition(Left, Top + MinHeight - 1);
            Console.Write(fullLine);

            for (var y = 1; y < MinHeight - 1; y++)
            {
                Console.SetCursorPosition(Left, Top + y);
                Console.Write(vertFill);
                Console.SetCursorPosition(Left + Width-1, Top + y);
                Console.Write(vertFill);
            }

            Console.ResetColor();
        }
    }
}