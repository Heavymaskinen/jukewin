using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ConsoleFrontend.Overlays
{
    public class FolderBrowserOverlay : Overlay
    {
        private DirectoryInfo currentDir;
        private string currentPath;
        private ConsoleMenu menu;
        private Background background;

        public FolderBrowserOverlay(string currentPath)
        {
            Top  = 0;
            Left = 0;
            background = new Background
            {
                BackgroundColor = ConsoleColor.DarkCyan,
                ForegroundColor = ConsoleColor.Red,
                Top             = 0,
                Left            = 0,
                MinHeight       = Console.WindowHeight,    
                Width           = Console.WindowWidth
            };
            ConsoleMenu.ItemSelected += OnMenuItemSelected;
            this.currentPath         =  currentPath;
            currentDir               =  new DirectoryInfo(currentPath);
            menu = new ConsoleMenu("Select a folder")
            {
                BackgroundColor = ConsoleColor.DarkCyan,
                SelectedColor   = ConsoleColor.DarkMagenta
            };

            ForegroundColor = ConsoleColor.Green;
            BackgroundColor = ConsoleColor.DarkCyan;

            AddComponent(background);
            AddComponent(menu);
            InitialiseMenu();
            Open();
        }

        private void OnMenuItemSelected(object? sender, string e)
        {
            if (e == "..")
            {
                currentDir  = currentDir.Parent;
                currentPath = currentDir != null ? currentDir.FullName : ".";
            }
            else
            {
                currentPath = e;
                currentDir  = new DirectoryInfo(currentPath);
            }

            InitialiseMenu();
        }

        private void InitialiseMenu()
        {
            menu.ClearItems();
            menu.Left = 0;
            menu.Top = 0;

            if (currentDir.Parent != null)
            {
                menu.AddItem(new ConsoleMenuItem() {ID = "..", Label = ".."});
            }

            try
            {
                var items = new Collection<string>(Directory.EnumerateDirectories(currentDir.FullName).ToArray());
                foreach (var s in items)
                {
                    menu.AddItem(new ConsoleMenuItem() {ID = s, Label = s});
                }

                background.AdjustHeight(items.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine("This is bad: " + e.Message);
            }
        }

        public override void UpdateInput(ConsoleKey key)
        {
            if (key == ConsoleKey.Escape)
            {
                Close();
            }
            else
            {
                menu.UpdateInput(key);
            }
        }
    }
}