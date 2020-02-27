using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ConsoleFrontend
{
    public class FolderBrowser
    {
        public event EventHandler<string> FolderSelected;
        private DirectoryInfo currentDir;

        private string currentPath;
        private ConsoleMenu menu;

        public FolderBrowser(string currentPath)
        {
            ConsoleMenu.ItemSelected += OnMenuItemSelected;
            this.currentPath         =  currentPath;
            currentDir               =  new DirectoryInfo(currentPath);
            InitialiseMenu();
            Show = true;
        }

        public string SelectedFolder { get; private set; }

        public bool Show { get; private set; }

        private void InitialiseMenu()
        {
            menu = new ConsoleMenu("Select a folder");
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
            }
            catch (Exception e)
            {
                Console.WriteLine("This is bad: " + e.Message);
            }
        }

        private void OnMenuItemSelected(object sender, string e)
        {
            if (e == "..")
            {
                currentDir  = currentDir.Parent;
                currentPath = currentDir.FullName;
            }
            else
            {
                currentPath = e;
                currentDir  = new DirectoryInfo(currentPath);
            }

            InitialiseMenu();
        }

        public void Draw()
        {
            menu.Draw();
        }

        public void UpdateInput(ConsoleKey key)
        {
            if (key == ConsoleKey.Escape)
            {
                Show = false;
            }

            if (key == ConsoleKey.Spacebar)
            {
                SelectedFolder = currentPath;
                Show           = false;
                FolderSelected?.Invoke(this, SelectedFolder);
            }
            else
            {
                menu.UpdateInput(key);
            }
        }
    }
}