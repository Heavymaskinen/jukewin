using System;
using System.IO;
using ConsoleFrontend.Overlays;
using CoreSongIO;
using Juke.Control;

namespace ConsoleFrontend.Screens
{
    public class AdminScreen : Screen
    {
        private Background background;
        private ConsoleMenu menu;
        private IJukeControl jukeControl;
        private string selectedFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        public AdminScreen(IJukeControl jukeControl, int width, int height)
        {
            this.jukeControl = jukeControl;
            menu = new ConsoleMenu("Admin controls",
                new ConsoleMenuItem() {ID = "Load", Label = "Load songs to library"},
                new ConsoleMenuItem() {ID = "Back", Label = "Back"}
            );

            menu.InstanceItemSelected += MenuOnItemSelected;

            background = new Background
            {
                BackgroundColor = ConsoleColor.DarkGray, ForegroundColor = ConsoleColor.Green, Width = width,
                MinHeight       = height
            };
            Width     = width;
            MinHeight = height;
        }

        private void MenuOnItemSelected(object sender, string id)
        {
            if (id == "Back")
            {
                ChangeScreen(ScreenName.MainMenu);
            }
            else if (id == "Load")
            {
                var browser =
                    new FolderBrowserOverlay(this,
                        selectedFolder,
                        Width - 2,
                        MinHeight
                    );
                browser.FolderSelected += OnFolderSelected;
                browser.Left           =  0;
                browser.Top            =  0;
                browser.Open();
            }
        }

        private void OnFolderSelected(object sender, string e)
        {
            GuiController.LogList.Add("Selected: "+e);
            selectedFolder = e;
            jukeControl.LoadHandler.LoadSongs(new CoreSongLoader("*mp3", selectedFolder));
            Messenger.Post("Songs added to library");
        }

        public override void Resized(int newWidth, int newHeight)
        {
            base.Resized(newWidth, newHeight);
            background.Width     = Width;
            background.MinHeight = MinHeight;
            menu.Width           = Width / 3;
            menu.Left            = Width / 2;
            Invalidate(true);
        }

        protected override void CustomRedraw()
        {
            background.Width     = Width;
            background.MinHeight = MinHeight;
            menu.Width           = Width / 3;
            menu.Left            = Width / 2;
            background.Draw();
            menu.Draw();
        }

        protected override void CustomDraw()
        {
            menu.MenuPart.SmallRedraw();
        }

        protected override void CustomUpdateInput(ConsoleKey key)
        {
            if (menu.UpdateInput(key))
            {
                Invalidate(false);
            }
            else
            {
                var isLetter = key >= ConsoleKey.A && key <= ConsoleKey.Z;
                var isNumber = key >= ConsoleKey.D1 && key <= ConsoleKey.D0;
                if (isLetter || isNumber || key == ConsoleKey.Spacebar)
                {
                    Invalidate(true);
                }
            }
        }
    }
}