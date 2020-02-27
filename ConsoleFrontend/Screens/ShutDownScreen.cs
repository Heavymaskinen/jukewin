using System;
using Juke.Control;

namespace ConsoleFrontend.Screens
{
    public class ShutDownScreen :Screen
    {
        private IJukeControl jukeControl;

        public ShutDownScreen(IJukeControl jukeControl)
        {
            this.jukeControl = jukeControl;
        }

        protected override void CustomRedraw()
        {
            Console.WriteLine("Shutting down - bye bye!");

            foreach (var s in GuiController.LogList)
            {
                Console.WriteLine(s);
            }
            
            jukeControl.Player.Stop();
            
            Environment.Exit(0);
        }

        protected override void CustomDraw()
        {
            CustomRedraw();
        }

        protected override void CustomUpdateInput(ConsoleKey key)
        {
            
        }
    }
}