using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ConsoleFrontend
{
    public class ConsoleMenu : IDrawable
    {
        public static event EventHandler<string> ItemSelected;

        private IList<ConsoleMenuItem> items;

        private int selectedIndex;

        public int Width
        {
            get => menuPart.Width;
            set
            {
                menuPart.Width  = value;
                titlePart.Width = value;
            }
        }

        public ConsoleColor BackgroundColor
        {
            get => menuPart.BackgroundColor;
            set
            {
                menuPart.BackgroundColor  = value;
                titlePart.BackgroundColor = value;
            }
        }

        public ConsoleColor ForegroundColor
        {
            get => menuPart.ForegroundColor;
            set
            {
                menuPart.ForegroundColor  = value;
                titlePart.ForegroundColor = value;
            }
        }

        public ConsoleColor SelectedColor
        {
            get => menuPart.SelectedBackgroundColor;
            set => menuPart.SelectedBackgroundColor = value;
        }

        public ConsoleColor FontColor { get; set; }

        public int Left
        {
            get => left;
            set
            {
                left           = value;
                titlePart.Left = value;
                menuPart.Left  = value;
            }
        }

        public int Top
        {
            get => top;
            set
            {
                top           = value;
                titlePart.Top = value;
                menuPart.Top  = value;
            }
        }

        public LineBlock TitlePart => titlePart;

        public LineBlock MenuPart => menuPart;

        public int MinHeight
        {
            get => menuPart.MinHeight;
            set => menuPart.MinHeight = value;
        }

        public ConsoleMenuItem SelectedItem => items[selectedIndex];

        private LineBlock titlePart;
        private LineBlock menuPart;
        private string title;
        private int left;
        private int top;

        public ConsoleMenu(string title, params ConsoleMenuItem[] items)
        {
            this.title    = title;
            left          = 0;
            top           = 0;
            this.items    = new List<ConsoleMenuItem>(items);
            selectedIndex = 0;
            var backCol = ConsoleColor.Black;
            var selCol  = ConsoleColor.DarkCyan;
            var foreCol = ConsoleColor.Gray;

            titlePart = new LineBlock
            {
                BackgroundColor = backCol, ForegroundColor = selCol, Left = Left, Top = Top,
                MinHeight       = 1
            };

            menuPart = new LineBlock
            {
                BackgroundColor = backCol, ForegroundColor = foreCol, SelectedBackgroundColor = selCol,
                Left            = Left
            };
        }

        private void BuildTitlePart()
        {
            titlePart.Clear();
            titlePart.AddLine(FillLine('-'));
            titlePart.AddLine(PadString(title));
            titlePart.AddLine(FillLine('-'));
            titlePart.AddLine(FillLine(' '));
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

        public void ClearItems()
        {
            items.Clear();
            menuPart.SelectedIndex = -1;
            selectedIndex          = 0;
        }

        private void Next()
        {
            if (selectedIndex < items.Count - 1)
            {
                selectedIndex++;
                menuPart.SelectedIndex++;
            }
        }

        private void Previous()
        {
            if (selectedIndex > 0)
            {
                selectedIndex--;
                menuPart.SelectedIndex--;
            }
        }

        public void Draw()
        {
            Console.SetCursorPosition(Left, Top);
            BuildTitlePart();
            titlePart.Draw();

            BuildMenuPart();
            menuPart.Draw();

            Console.ResetColor();
        }

        private void BuildMenuPart()
        {
            menuPart.Clear();
            foreach (var item in items)
            {
                menuPart.AddLine(item.Label);
            }

            menuPart.SelectedIndex = selectedIndex;
            menuPart.Top           = Top + titlePart.Height;
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
            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }
        }
    }
}