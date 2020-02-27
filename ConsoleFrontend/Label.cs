using System;

namespace ConsoleFrontend
{
    public class Label : TextElement
    {
        private string text;
        public int Top { get; set; }
        public int Left { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public int Width
        {
            get => text.Length;
            set
            {
                if (text.Length > value)
                {
                    text = text.Remove(text.Length - value - 1);
                }
                else
                {
                    while (text.Length < value)
                    {
                        text += " ";
                    }
                }
            }
        }

        public int MinHeight { get; set; }

        public string Text => text;

        public Label(string text)
        {
            this.text = text;
            MinHeight = 1;
        }

        public void AppendText(string txt)
        {
            text += txt;
        }

        public void ClearText()
        {
            text = "";
        }

        public void SetText(string txt)
        {
            text = txt;
        }

        public void Draw()
        {
            Console.SetCursorPosition(Left,Top);
            Console.ForegroundColor = ForegroundColor;
            Console.BackgroundColor = BackgroundColor;
            Console.Write(Text);
            Console.ResetColor();
        }
    }
}