using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ConsoleFrontend
{
    public class MultiLineText : TextElement
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public int Width
        {
            get
            {
                return lines.Select(l => l.Length).Concat(new[] {0}).Max();
            }
            set
            {
                
            }
        }

        public int MinHeight
        {
            get => lines.Count;
            set => MaxLines = value;
        }

        public string Text
        {
            get { return lines.Aggregate("", (current, s) => current + (s + "\n")); }
        }

        public int MaxLines { get; set; }
        private IList<string> lines;

        public MultiLineText()
        {
            MaxLines = 5;
            lines    = new Collection<string>();
        }

        public void AppendText(string txt)
        {
            lines.Add(txt);
            if (lines.Count > MaxLines)
            {
                lines.RemoveAt(0);
            }
        }

        public void ClearText()
        {
            lines.Clear();
        }

        public void SetText(string txt)
        {
            lines.Clear();
            lines.Add(txt);
        }

        public void Draw()
        {
            Console.ForegroundColor = ForegroundColor;
            Console.BackgroundColor = BackgroundColor;
            for (var i = 0; i < lines.Count; i++)
            {
                Console.SetCursorPosition(Left, Top+i);
                Console.Write(lines[i]);
            }
            
            Console.ResetColor();
        }
    }
}