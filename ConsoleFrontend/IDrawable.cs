using System;

namespace ConsoleFrontend
{
    public interface IDrawable
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public void Draw();
    }
}