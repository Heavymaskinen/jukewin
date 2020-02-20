using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleFrontend
{
    public class ConsoleMenu
    {
        public static event EventHandler<string> ItemSelected;

        private IList<ConsoleMenuItem> items;
        private int selectedIndex;
        private string title;
        public int Width { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor SelectedColor { get; set; }
        public ConsoleColor FontColor { get; set; }
        
        public ConsoleMenuItem SelectedItem => items[selectedIndex];

        public ConsoleMenu(string title, params ConsoleMenuItem[] items)
        {
            Width = 50;
            this.title = title;
            this.items = new List<ConsoleMenuItem>(items);
            selectedIndex = 0;
            BackgroundColor = ConsoleColor.Black;
            SelectedColor = ConsoleColor.DarkCyan;
            FontColor = ConsoleColor.White;
        }

        public void UpdateInput(ConsoleKey key)
        {
            if (key == ConsoleKey.UpArrow)
            {
                Previous();
            }

            if (key == ConsoleKey.DownArrow)
            {
                Next();
            }

            if (key == ConsoleKey.Enter)
            {
                ItemSelected?.Invoke(this, items[selectedIndex].ID);
            }
        }

        private void Next()
        {
            if (selectedIndex < items.Count - 1)
            {
                selectedIndex++;
            }
        }

        private void Previous()
        {
            if (selectedIndex > 0)
            {
                selectedIndex--;
            }
        }

        public void Draw()
        {
            Console.WriteLine(FillLine('-'));
            Console.WriteLine(title);
            Console.WriteLine(FillLine('-'));
            
            var originalForeground = Console.ForegroundColor;
            var originalBackground = Console.BackgroundColor;
            Console.ForegroundColor = FontColor;
            for (var i = 0; i < items.Count; i++)
            {
                if (selectedIndex == i)
                {
                    Console.BackgroundColor = SelectedColor;
                }
                else
                {
                    Console.BackgroundColor = BackgroundColor;
                }

                Console.WriteLine(PadString(items[i].Label));
            }

            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;
        }

        private string PadString(string txt)
        {
            var str = new StringBuilder(txt);
            for (var i = txt.Length; i < Width; i++)
            {
                str.Append(" ");
            }

            return str.ToString();
        }

        private string FillLine(char c)
        {
            var str = new StringBuilder();
            for (var i = 0; i < Width; i++)
            {
                str.Append(c);
            }

            return str.ToString();
        }

        public void AddItem(ConsoleMenuItem item)
        {
            items.Add(item);
        }
    }
}