using System;
using ConsoleFrontend.Overlays;

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
                new ConsoleMenuItem {ID = "Library", Label                   = "Show library"},
                new ConsoleMenuItem {ID = "Queue", Label                     = "Show queue"},
                new ConsoleMenuItem {ID = "Player", Label                    = "Show player"},
                new ConsoleMenuItem {ID = ScreenName.Admin.ToString(), Label = "Admin controls"},
                new ConsoleMenuItem {ID = "Quit", Label                      = "Quit"}
            )
            {
                Width           = width / 3,
                Left            = width / 2, Top = 0,
                BackgroundColor = ConsoleColor.Black,
                ForegroundColor = ConsoleColor.White,
                SelectedColor   = ConsoleColor.Green
            };

            menu.InstanceItemSelected += MenuOnItemSelected;

            Width     = width;
            MinHeight = height;
        }

        private void MenuOnItemSelected(object sender, string name)
        {
            ChangeScreen(name);
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
            if (key == ConsoleKey.P)
            {
                var prompt = new PromptOverlay(this, "Write here")
                    {ForegroundColor = ConsoleColor.White, BackgroundColor = ConsoleColor.Black};
                prompt.Open();
                return;
            }

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