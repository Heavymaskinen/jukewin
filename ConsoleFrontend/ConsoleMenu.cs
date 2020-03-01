using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;
using MessageRouting;
using TagLib.Riff;

namespace ConsoleFrontend
{
    public class ConsoleMenu : IDrawable
    {
        public static event EventHandler<string> ItemSelected;
        public event EventHandler<string> InstanceItemSelected;

        private List<ConsoleMenuItem> items;

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
                menuPart.Top  = value + titlePart.MinHeight;
            }
        }

        public IDrawable TitlePart => titlePart;

        public LineBlock MenuPart => menuPart;


        public int MinHeight
        {
            get => menuPart.MinHeight;
            set => menuPart.MinHeight = value;
        }

        public int ItemCount => items.Count;

        private TextBox titlePart;
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

            titlePart = new TextBox(title, '|', '-', 10);

            menuPart = new LineBlock
            {
                BackgroundColor = backCol, ForegroundColor = foreCol, SelectedBackgroundColor = selCol,
                Left            = Left
            };
        }

        private void BuildTitlePart()
        {
            titlePart.Top             = top;
            titlePart.Left            = Left;
            titlePart.Width           = Width;
            titlePart.ForegroundColor = ForegroundColor;
            titlePart.BackgroundColor = BackgroundColor;
        }

        public bool UpdateInput(ConsoleKey key)
        {
            if (key == ConsoleKey.UpArrow)
            {
                Previous();
                return true;
            }

            if (key == ConsoleKey.DownArrow)
            {
                Next();
                return true;
            }

            if (key == ConsoleKey.Enter)
            {
                ItemSelected?.Invoke(this, items[selectedIndex].ID);
                InstanceItemSelected?.Invoke(this, items[selectedIndex].ID);
            }

            return false;
        }

        public void ClearItems()
        {
            items.Clear();
            menuPart.SelectedIndex = 0;
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

        public void ReducedDraw()
        {
            menuPart.SmallRedraw();
        }

        private void BuildMenuPart()
        {
            menuPart.Clear();
            foreach (var item in items)
            {
                menuPart.AddLine(item.Label);
            }

            menuPart.SelectedIndex = selectedIndex;
        }

        public void AddItem(ConsoleMenuItem item)
        {
            items.Add(item);
            items.Sort();
            
            if (selectedIndex < 0)
            {
                menuPart.SelectedIndex = 0;
                selectedIndex          = 0;
            }
        }
    }
}