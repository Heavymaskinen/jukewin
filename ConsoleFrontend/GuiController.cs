using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Juke.Control;

namespace ConsoleFrontend
{
    public class GuiController
    {
        private IDictionary<string, ConsoleMenu> menus;
        private ConsoleMenu activeMenu;
        private Keyboard keyboard;

        private IDictionary<string, Action> menuMapping;
        private IJukeControl jukeControl;
        private FolderBrowser overlay;

        public GuiController(IJukeControl jukeControl)
        {
            this.jukeControl = jukeControl;
            overlay = null;

            menuMapping = new Dictionary<string, Action>();

            var mainMenu = new ConsoleMenu("JUKE - Main Menu",
                new ConsoleMenuItem() {ID = "Library", Label = "Show library"},
                new ConsoleMenuItem() {ID = "Queue", Label = "Show queue"},
                new ConsoleMenuItem() {ID = "Player", Label = "Show player"},
                new ConsoleMenuItem() {ID = "Admin", Label = "Admin controls"},
                new ConsoleMenuItem() {ID = "Exit", Label = "Quit"}
            );

            var libMenu = new ConsoleMenu("JUKE - Library",
                new ConsoleMenuItem() {ID = "Artists", Label = "List artists"},
                new ConsoleMenuItem() {ID = "Songs", Label = "List songs"},
                new ConsoleMenuItem() {ID = "Search", Label = "Search"},
                new ConsoleMenuItem() {ID = "Play", Label = "Play a song"},
                new ConsoleMenuItem() {ID = "MainMenu", Label = "Back to main menu"}
            );

            var adminMenu = new ConsoleMenu("JUKE - Admin Controls",
                new ConsoleMenuItem() {ID = "AddSongs", Label = "Add songs from folder"},
                new ConsoleMenuItem() {ID = "MainMenu", Label = "Back to main menu"}
            );

            menus = new Dictionary<string, ConsoleMenu>()
            {
                {"main", mainMenu},
                {"lib", libMenu},
                {"admin", adminMenu}
            };

            menuMapping["MainMenu"] = () => ChangeMenu("main");
            menuMapping["Library"] = () => ChangeMenu("lib");
            menuMapping["Admin"] = () => ChangeMenu("admin");
            menuMapping["Songs"] = RenderSongList;
            menuMapping["Exit"] = ShutDown;
            menuMapping["AddSongs"] = () =>
            {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                overlay = new FolderBrowser(dir);
                overlay.FolderSelected += OnFolderSelected;
                Render();
            };
            
            ConsoleMenu.ItemSelected += OnMainItemSelected;
            activeMenu = mainMenu;


            Console.CursorVisible = false;
            keyboard = new Keyboard();
            keyboard.KeyPressed += OnKeyPressed;
        }

        private void OnFolderSelected(object? sender, string e)
        {
            Render();
        }

        private void ShutDown()
        {
            keyboard.Stop();
            Console.WriteLine("Bye bye!");
            Environment.Exit(0);
        }

        public void Run()
        {
            Render();
            keyboard.Listen();
            while (true)
            {
                Thread.Sleep(10);
                if (overlay != null && !overlay.Show)
                {
                    overlay = null;
                }
            }
        }

        private void RenderSongList()
        {
            foreach (var song in jukeControl.Browser.Songs)
            {
                Console.WriteLine(song.ToString());
            }
        }

        private void ChangeMenu(string name)
        {
            activeMenu = menus[name];
            Render();
        }

        private void Render()
        {
            Console.Clear();
            if (overlay != null)
            {
                overlay.Draw();
            }
            else
            {
                activeMenu.Draw();
            }
        }

        private void OnKeyPressed(object sender, ConsoleKey key)
        {
            if (overlay != null)
            {
                overlay.UpdateInput(key);
            }
            else
            {
                activeMenu.UpdateInput(key);
            }

            Render();
        }

        private void OnMainItemSelected(object sender, string id)
        {
            if (menuMapping.ContainsKey(id))
            {
                menuMapping[id]();
            }
        }
    }
}