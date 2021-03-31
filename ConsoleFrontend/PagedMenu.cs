using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Juke.Control;

namespace ConsoleFrontend
{
    public class PagedMenu : IDrawable
    {
        public interface IPopulator
        {
            public void ItemSelected(string name);
            public IList<ConsoleMenuItem> GetPreItems();
        }

        private ConsoleMenu consoleMenu;
        private IList<ConsoleMenuItem> items;
        private int startIndex;
        private int endIndex;
        public int PageSize { get; }

        public int Top { get; set; }
        public int Left { get; set; }

        public IPopulator ItemPopulator { get; set; }

        public ConsoleColor BackgroundColor
        {
            get => Menu.BackgroundColor;
            set => Menu.BackgroundColor = value;
        }

        public ConsoleColor ForegroundColor
        {
            get => Menu.ForegroundColor;
            set => Menu.ForegroundColor = value;
        }

        public int Width
        {
            get => Menu.Width;
            set => Menu.Width = value;
        }

        public int MinHeight
        {
            get => Menu.MinHeight;
            set => Menu.MinHeight = value;
        }

        public ConsoleMenu Menu => consoleMenu;

        public PagedMenu(string title, IList<ConsoleMenuItem> items)
        {
            this.items                       =  items;
            consoleMenu                      =  new ConsoleMenu(title, items.ToArray());
            startIndex                       =  0;
            endIndex                         =  Math.Min(items.Count, startIndex + MinHeight);
            consoleMenu.InstanceItemSelected += ConsoleMenuOnInstanceItemSelected;
            ForegroundColor                  =  ConsoleColor.White;
            PageSize                         =  15;
        }

        private void ConsoleMenuOnInstanceItemSelected(object sender, string e)
        {
            GuiController.SLog(e + " selected!");
            if (ItemPopulator != null)
            {
                GuiController.SLog("Populate");
                ItemPopulator.ItemSelected(e);
            }
        }

        public void Draw()
        {
            consoleMenu.MinHeight = PageSize;
            consoleMenu.Draw();
        }

        public void ReducedDraw()
        {
            consoleMenu.MinHeight = PageSize;
            consoleMenu.Draw();
        }

        public void NextPage()
        {
            if (endIndex < items.Count)
            {
                GuiController.SLog("Next page!");
                startIndex += PageSize;
                endIndex   =  Math.Min(items.Count, startIndex + MinHeight);
                InitialiseMenu();
            }
        }

        public void PreviousPage()
        {
            if (startIndex > 0)
            {
                GuiController.SLog("Previous page!");
                startIndex = Math.Max(0, startIndex - PageSize);
                endIndex   = Math.Min(items.Count, endIndex - PageSize);
                InitialiseMenu();
            }
        }

        private void InitialiseMenu()
        {
            Menu.ClearItems();
            Menu.Left      = Left;
            Menu.Top       = Top;
            Menu.MinHeight = PageSize;

            if (ItemPopulator != null)
            {
                var firstItems = ItemPopulator.GetPreItems();
                foreach (var item in firstItems)
                {
                    Menu.AddItem(item);
                }
            }

            var maxIndex = endIndex - Menu.ItemCount;
            try
            {
                GuiController.SLog("Menu height: " + Menu.MinHeight);
                GuiController.SLog("Add " + startIndex + " to " + maxIndex+" out of "+items.Count);
                for (var i = startIndex; i < items.Count && i <= maxIndex; i++)
                {
                    var s = items[i];
                    Menu.AddItem(items[i]);
                }
            }
            catch (Exception e)
            {
                Messenger.Post("This is bad: " + e.Message);
            }
        }

        public void SetItems(Collection<ConsoleMenuItem> consoleMenuItems)
        {
            items      = consoleMenuItems;
            startIndex = 0;
            endIndex   = Math.Min(items.Count, startIndex + MinHeight);
            InitialiseMenu();
        }

        public bool UpdateInput(ConsoleKey key)
        {
            if (key == ConsoleKey.W)
            {
                NextPage();
                InitialiseMenu();
                return true;
            }

            if (key == ConsoleKey.Q)
            {
                PreviousPage();
                InitialiseMenu();
                return true;
            }

            return consoleMenu.UpdateInput(key);
        }
    }
}