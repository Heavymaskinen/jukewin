using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using MessageRouting;
using Microsoft.VisualBasic;

namespace ConsoleFrontend.Overlays
{
    public class FolderBrowserOverlay : Overlay, PagedMenu.IPopulator
    {
        public event EventHandler<string> FolderSelected;

        private DirectoryInfo currentDir;
        private string currentPath;
        private ConsoleMenu menu;
        private Background background;
        private Label label;
        private PagedMenu pagedMenu;

        private int startLine;
        private int endLine;

        public string SelectedFolder { get; set; }

        public FolderBrowserOverlay(Screen parent, string currentPath, int width, int height) : base(parent)
        {
            label = new Label(currentPath)
            {
                ForegroundColor = ConsoleColor.Green, BackgroundColor = ConsoleColor.Black,
                Top             = Top,
                Left            = Left
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

            pagedMenu = new PagedMenu("Select a folder", GetSubDirsAsMenuItems())
            {
                BackgroundColor = ConsoleColor.DarkGray,
                ForegroundColor = ConsoleColor.DarkGreen,
                Top             = Top + 10,
                Left            = Left,
                ItemPopulator   = this
            };

            ItemSelected(currentPath);

            ForegroundColor = ConsoleColor.Green;
            BackgroundColor = ConsoleColor.DarkCyan;

            AddComponent(background);
            AddComponent(pagedMenu);
            AddComponent(label);
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
                pagedMenu.UpdateInput(key);
                Invalidate(false);
            }
        }

        public void ItemSelected(string e)
        {
            GuiController.SLog("Item selected()");
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

            try
            {
                var items = GetSubDirsAsMenuItems();
                pagedMenu.SetItems(items);
            }
            catch (Exception ex)
            {
                Messenger.Post("This is bad: " + ex.Message);
            }
        }

        private Collection<ConsoleMenuItem> GetSubDirsAsMenuItems()
        {
            GuiController.SLog("Fetching for "+currentDir.FullName);
            var dirs  = new Collection<string>(Directory.EnumerateDirectories(currentDir.FullName).ToArray());
            GuiController.SLog("Found "+dirs.Count);
            var items = new Collection<ConsoleMenuItem>();
            foreach (var s in dirs)
            {
                items.Add(new ConsoleMenuItem {ID = s, Label = new DirectoryInfo(s).Name});
            }

            return items;
        }

        public IList<ConsoleMenuItem> GetPreItems()
        {
            if (currentDir.Parent != null)
            {
                return new Collection<ConsoleMenuItem> {new ConsoleMenuItem() {ID = "..", Label = ".."}};
            }

            return new Collection<ConsoleMenuItem>();
        }
    }
}