using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ConsoleFrontend.Overlays
{
    public class FolderBrowserOverlay : Overlay
    {
        public event EventHandler<string> FolderSelected;

        private DirectoryInfo currentDir;
        private string currentPath;
        private ConsoleMenu menu;
        private Background background;
        private Label label;

        private int startLine;
        private int endLine;

        public string SelectedFolder { get; set; }

        public FolderBrowserOverlay(Screen parent, string currentPath, int width, int height) : base(parent)
        {
            label = new Label(currentPath)
            {
                ForegroundColor = ConsoleColor.Green, BackgroundColor = ConsoleColor.Black,
                Top  = Top,
                Left = Left
            };
            background = new Background
            {
                BackgroundColor = ConsoleColor.Black,
                ForegroundColor = ConsoleColor.Green,
                Top             = 0,
                Left            = 0,
                MinHeight       = Console.WindowHeight,
                Width           = Console.WindowWidth
            };
            this.currentPath = currentPath;
            currentDir       = new DirectoryInfo(currentPath);
            menu = new ConsoleMenu("Select a folder")
            {
                BackgroundColor = ConsoleColor.DarkGray,
                SelectedColor   = ConsoleColor.DarkGreen,
                Top             = Top + 10,
                Left            = Left
            };
            menu.InstanceItemSelected += OnMenuItemSelected;

            ForegroundColor = ConsoleColor.Green;
            BackgroundColor = ConsoleColor.DarkCyan;
            startLine       = 0;
            endLine         = height - menu.Top;

            AddComponent(background);
            AddComponent(menu);
            AddComponent(label);
            InitialiseMenu();
        }

        private void OnMenuItemSelected(object sender, string e)
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

            label.SetText(currentPath);
            GuiController.LogList.Add("Path: " + currentPath);

            InitialiseMenu();
        }

        private void InitialiseMenu()
        {
            menu.ClearItems();
            menu.Left = Left;
            menu.Top  = Top;

            if (currentDir.Parent != null)
            {
                menu.AddItem(new ConsoleMenuItem() {ID = "..", Label = ".."});
            }

            try
            {
                var items = new Collection<string>(Directory.EnumerateDirectories(currentDir.FullName).ToArray());
                for (var i = startLine; i < items.Count && i < endLine; i++)
                {
                    var s = items[i];
                    menu.AddItem(new ConsoleMenuItem {ID = s, Label = new DirectoryInfo(s).Name});
                }

                background.AdjustHeight(items.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine("This is bad: " + e.Message);
            }
        }

        protected override void CustomInputHandle(ConsoleKey key)
        {
            if (key == ConsoleKey.Spacebar)
            {
                SelectedFolder = currentPath;
                FolderSelected?.Invoke(this, SelectedFolder);
                Close();
            }
            else
            {
                menu.UpdateInput(key);
                Invalidate(false);
            }
        }
    }
}