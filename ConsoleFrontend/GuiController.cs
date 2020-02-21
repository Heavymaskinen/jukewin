using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using ConsoleFrontend.Overlays;
using ConsoleFrontend.Screens;
using Juke.Control;

namespace ConsoleFrontend
{
    public class GuiController
    {
        private IDictionary<string, ConsoleMenu> menus;
        private ConsoleMenu activeMenu;
        private Keyboard keyboard;

        private IList<LineBlock> drawables;
        private IDictionary<string, Action> menuMapping;
        private IJukeControl jukeControl;
        private FolderBrowser overlay;

        private IDictionary<ScreenName, Screen> sceneMap;
        private Screen activeScreen;

        private Overlay dialog;

        public GuiController(IJukeControl jukeControl)
        {
            this.jukeControl = jukeControl;
            overlay          = null;
            dialog           = null;
            menuMapping      = new Dictionary<string, Action>();

            Overlay.Opened     += OverlayOnOpened;
            Overlay.Closed     += OverlayOnClosed;
            Screen.Invalidated += ScreenOnInvalidated;
            
            sceneMap = new Dictionary<ScreenName, Screen>
            {
                {ScreenName.MainMenu, new MainMenuScreen(Console.WindowWidth, Console.WindowHeight)}
            };

            activeScreen = sceneMap[ScreenName.MainMenu];

            var mainMenu = new ConsoleMenu("JUKE - Main Menu",
                new ConsoleMenuItem() {ID = "Library", Label = "Show library"},
                new ConsoleMenuItem() {ID = "Queue", Label   = "Show queue"},
                new ConsoleMenuItem() {ID = "Player", Label  = "Show player"},
                new ConsoleMenuItem() {ID = "Admin", Label   = "Admin controls"},
                new ConsoleMenuItem() {ID = "Exit", Label    = "Quit"}
            ) {Width = Console.WindowWidth};

            var libMenu = new ConsoleMenu("JUKE - Library",
                new ConsoleMenuItem() {ID = "Artists", Label  = "List artists"},
                new ConsoleMenuItem() {ID = "Songs", Label    = "List songs"},
                new ConsoleMenuItem() {ID = "Search", Label   = "Search"},
                new ConsoleMenuItem() {ID = "Play", Label     = "Play a song"},
                new ConsoleMenuItem() {ID = "MainMenu", Label = "Back to main menu"}
            ) {Width = Console.WindowWidth};

            var adminMenu = new ConsoleMenu("JUKE - Admin Controls",
                new ConsoleMenuItem() {ID = "AddSongs", Label = "Add songs from folder"},
                new ConsoleMenuItem() {ID = "MainMenu", Label = "Back to main menu"}
            ) {Width = Console.WindowWidth};

            menus = new Dictionary<string, ConsoleMenu>()
            {
                {"main", mainMenu},
                {"lib", libMenu},
                {"admin", adminMenu}
            };

            menuMapping["MainMenu"] = () => ChangeMenu("main");
            menuMapping["Library"]  = () => ChangeMenu("lib");
            menuMapping["Admin"]    = () => ChangeMenu("admin");
            menuMapping["Songs"]    = RenderSongList;
            menuMapping["Exit"]     = ShutDown;
            menuMapping["AddSongs"] = () =>
            {
                var dir    = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                var folder = new FolderBrowserOverlay(dir);
            };

            ConsoleMenu.ItemSelected += OnMainItemSelected;
            activeMenu               =  mainMenu;

            Console.CursorVisible =  false;
            keyboard              =  new Keyboard();
            keyboard.KeyPressed   += OnKeyPressed;

            drawables = new Collection<LineBlock>();
        }

        private void ScreenOnInvalidated(object sender, EventArgs e)
        {
            Render();
        }

        private void OverlayOnOpened(object sender, EventArgs e)
        {
            dialog = (Overlay) sender;
            Render();
        }

        private void OverlayOnClosed(object sender, EventArgs e)
        {
            dialog = null;
            Render();
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
                if (activeScreen.Width != Console.WindowWidth ||
                    activeScreen.MinHeight != Console.WindowHeight)
                {
                    activeScreen.Width = Console.WindowWidth;
                    activeScreen.MinHeight = Console.WindowHeight;
                    Render();
                }
            }
        }

        private void RenderSongList()
        {
            var songList = new LineBlock
            {
                Left            = Console.WindowWidth/2,
                BackgroundColor = ConsoleColor.DarkBlue,
                ForegroundColor = ConsoleColor.Cyan
            };
            songList.Background.BackgroundColor = ConsoleColor.DarkBlue;

            songList.AddLine("Songs in library:");
            songList.AddLine("-----------------");
            foreach (var song in jukeControl.Browser.Songs)
            {
                songList.AddLine(song.ToString());
            }

            drawables.Add(songList);
        }

        private void ChangeMenu(string name)
        {
            activeMenu = menus[name];
            Render();
        }

        private void Render()
        {
            //Console.Clear();
            if (dialog != null)
            {
                dialog.Draw();
            }
            else
            {
                activeScreen.Draw();
            }
        }

        private void OnKeyPressed(object sender, ConsoleKey key)
        {
            if (dialog != null)
            {
                dialog.UpdateInput(key);
            }
            else
            {
                activeScreen.UpdateInput(key);
            }

            Render();
        }

        private void OnMainItemSelected(object sender, string id)
        {
            if (menuMapping.ContainsKey(id))
            {
                drawables.Clear();
                menuMapping[id]();
            }
        }
    }
}