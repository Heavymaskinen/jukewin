using System;

namespace ConsoleFrontend.Screens
{
    public class MainMenuScreen : Screen
    {
        private ConsoleMenu menu;
        private Background background;

        public MainMenuScreen(int width, int height)
        {
            background = new Background {BackgroundColor = ConsoleColor.DarkBlue, Width = width, MinHeight = height};

            menu = new ConsoleMenu("JUKE - Main Menu",
                new ConsoleMenuItem() {ID = "Library", Label = "Show library"},
                new ConsoleMenuItem() {ID = "Queue", Label   = "Show queue"},
                new ConsoleMenuItem() {ID = "Player", Label  = "Show player"},
                new ConsoleMenuItem() {ID = "Admin", Label   = "Admin controls"},
                new ConsoleMenuItem() {ID = "Exit", Label    = "Quit"}
            )
            {
                Width           = width / 3, Left = width / 2, Top = 0,
                BackgroundColor = ConsoleColor.Black,
                ForegroundColor       = ConsoleColor.White, SelectedColor = ConsoleColor.Green
            };

            Width     = width;
            MinHeight = height;
        }

        public override void Draw()
        {
            background.Draw();
            menu.Draw();
        }

        public override void UpdateInput(ConsoleKey key)
        {
            menu.UpdateInput(key);
        }
    }
}