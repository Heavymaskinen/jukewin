using System;
using System.Text;

namespace ConsoleFrontend
{
    public class Background : IDrawable
    {
        public int MinHeight { get; set; }
        public int Top { get; set; }
        public int Left { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public int Width { get; set; }

        public void AdjustHeight(int other)
        {
            MinHeight = Math.Max(MinHeight, other);
        }

        public void Draw()
        {
            SetColors();
            var padding = CreatePaddingLine();
            DrawPaddingLines(padding);
            Console.ResetColor();
        }

        private string CreatePaddingLine()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < Width; i++)
            {
                builder.Append(" ");
            }

            var padding = builder.ToString();
            return padding;
        }

        private void DrawPaddingLines(string padding)
        {
            var count = 0;
            for (var y = 0; y < MinHeight; y++)
            {
                Console.SetCursorPosition(Left, Top+count);
                Console.Write(padding);
                count++;
            }
        }

        private void SetColors()
        {
            Console.ForegroundColor = ForegroundColor;
            Console.BackgroundColor = BackgroundColor;
        }
    }
}