using System;
using System.Threading.Tasks;
using MessageRouting;

namespace ConsoleFrontend
{
    public class Renderer
    {
        private MessageBox messageBox;

        public Renderer()
        {
            Screen.Invalidated += ScreenOnInvalidated;
            messageBox = new MessageBox(5)
            {
                ForegroundColor = ConsoleColor.Green, BackgroundColor = ConsoleColor.DarkGray
            };
            Messenger.Post("Welcome to JUKE!");
            Messenger.Post("Welcome to JUKE!");
            Messenger.Post("Welcome to JUKE!");
        }

        private void ScreenOnInvalidated(object sender, bool e)
        {
            var screen = (Screen) sender;
            if (e)
            {
                Console.Clear();
                screen.Redraw();
                if (!screen.HasOverlay)
                {
                    messageBox.Top  = Console.WindowHeight - 10;
                    messageBox.Left = 10;
                    messageBox.Draw();
                }
            }
            else
            {
                screen.Draw();
            }
        }
    }
}