using System;
using System.Threading.Tasks;

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
                GuiController.LogList.Add("Full render " + screen + ", " + screen.Width + " , " + screen.MinHeight);
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
                GuiController.LogList.Add("Small render " + screen + ", " + screen.Width + " , " + screen.MinHeight);
                screen.Draw();
            }
        }
    }
}