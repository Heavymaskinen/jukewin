using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using ConsoleFrontend.Screens;
using Juke.Control;

namespace ConsoleFrontend
{
    public class GuiController
    {
        public static List<string> LogList = new List<string>();

        public static void SLog(string log)
        {
            LogList.Add(log);
        }

        private Keyboard keyboard;

        private IList<LineBlock> drawables;
        private IDictionary<string, Action> menuMapping;
        private IJukeController jukeControl;
        private FolderBrowser overlay;

        private IDictionary<ScreenName, Screen> sceneMap;
        private Screen activeScreen;
        private bool paint = false;
        private bool invalidate = false;
        private Queue<bool> renderQueue = new Queue<bool>();
        private bool rendering = false;
        private Window window;
        private Renderer renderer;

        public GuiController(IJukeController jukeControl)
        {
            this.jukeControl = jukeControl;
            overlay          = null;
            menuMapping      = new Dictionary<string, Action>();

            //Overlay.Opened     += OverlayOnOpened;
            //Overlay.Closed     += OverlayOnClosed;
            //Screen.Invalidated  += ScreenOnInvalidated;
            Screen.ScreenChange  += OnScreenChange;
            Window.WindowResized += WindowOnWindowResized;
            window               =  new Window();
            window.Run();
            sceneMap = new Dictionary<ScreenName, Screen>
            {
                {ScreenName.MainMenu, new MainMenuScreen(Console.WindowWidth, Console.WindowHeight)},
                {ScreenName.Admin, new AdminScreen(jukeControl, Console.WindowWidth, Console.WindowHeight)},
                {ScreenName.Library, new LibraryScreen(jukeControl, Console.WindowWidth, Console.WindowHeight)},
                {ScreenName.Quit, new ShutDownScreen(jukeControl)}
            };

            activeScreen = sceneMap[ScreenName.MainMenu];

            Console.CursorVisible =  false;
            Keyboard.KeyPressed   += OnKeyPressed;
            keyboard              =  new Keyboard();

            drawables = new Collection<LineBlock>();
            renderer  = new Renderer();
            Console.Title = "J.U.K.E. CLI version";
        }

        private void WindowOnWindowResized(object sender, EventArgs e)
        {
            Console.Clear();
            activeScreen.Resized(Console.WindowWidth, Console.WindowHeight);
        }

        private void Log(string txt)
        {
            LogList.Add(txt);
        }

        private void OnScreenChange(object sender, ScreenName sceneName)
        {
            Log("Screen change! " + sceneName);
            if (!sceneMap.ContainsKey(sceneName)) return;
            activeScreen.Visible = false;
            var oldScreen = activeScreen;
            activeScreen           = sceneMap[sceneName];
            activeScreen.Width     = oldScreen.Width;
            activeScreen.MinHeight = oldScreen.MinHeight;
            activeScreen.Visible   = true;

            activeScreen.Invalidate(true);
        }

        public void Run()
        {
            activeScreen.Visible = true;
            activeScreen.Invalidate(true);
            keyboard.Listen();
            while (true)
            {
                Thread.Sleep(10);
            }
        }

        private void OnKeyPressed(object sender, ConsoleKey key)
        {
            if (key == ConsoleKey.R)
            {
                activeScreen.Invalidate(true);
            }

            activeScreen.UpdateInput(key);
        }
    }
}