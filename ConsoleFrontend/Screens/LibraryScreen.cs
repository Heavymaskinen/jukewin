using System;
using System.IO;
using ConsoleFrontend.Overlays;
using CoreSongIO;
using Juke.Control;
using TagLib;

namespace ConsoleFrontend.Screens
{
    public class LibraryScreen : Screen
    {
        private ConsoleMenu menu;
        private IJukeControl jukeControl;

        public LibraryScreen(IJukeControl jukeControl, int width, int height)
        {
            this.jukeControl = jukeControl;
            menu = new ConsoleMenu("Library",
                new ConsoleMenuItem {ID = "Songs", Label = "List songs"},
                new ConsoleMenuItem {ID = "Load", Label  = "Load library"},
                new ConsoleMenuItem {ID = "Save", Label  = "Save library"},
                new ConsoleMenuItem {ID = "Back", Label  = "Back"}
            ) {Width = width, MinHeight = height};

            menu.InstanceItemSelected += MenuOnInstanceItemSelected;
            Width                     =  width;
            MinHeight                 =  height;
        }

        private void MenuOnInstanceItemSelected(object sender, string id)
        {
            switch (id)
            {
                case "Back":
                    ChangeScreen(ScreenName.MainMenu);
                    break;
                case "Songs":
                    var overlay = new SongBrowserOverlay(this, jukeControl.Browser.Songs);
                    overlay.SongSelected += (o, songTitle) => PlaySong(songTitle);
                    overlay.Open();
                    break;
                case "Save":

                    var prompt = new PromptOverlay(this, "Choose filename");
                    prompt.TextEntered += SaveFileSelected; 
                    prompt.Open();
                    break;
            }
        }

        private void SaveFileSelected(object? sender, string filename)
        {
            var musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            var totalName = musicFolder + "//" + filename + ".xml";
            var io = new LibraryIO(filename);
            jukeControl.SaveHandler.SaveSongs(io);
            Messenger.Post("Library saved to "+totalName);
        }
        

        private void PlaySong(string songTitle)
        {
            var song = jukeControl.Browser.GetSongsByTitle(songTitle)[0];
            jukeControl.Player.PlaySong(song);
            Console.Title = "J.U.K.E. playing " + song.Artist + " - " + song.Name;
        }

        protected override void CustomRedraw()
        {
            menu.Draw();
        }

        protected override void CustomDraw()
        {
            menu.ReducedDraw();
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