using System;
using System.IO;
using System.Threading.Tasks;
using ConsoleFrontend.Screens;
using Juke.Control;

namespace ConsoleFrontend
{
    class Program
    {
        static void Main(string[] args)
        {
            var jukeControl = JukeController.Instance;
            jukeControl.LoadHandler.LoadSongs(new FakeLoader());
            var gui = new GuiController(jukeControl);

            //Debug();
            gui.Run();
        }

        private static void Debug()
        {
            Console.CursorVisible = false;
            Console.Clear();
            var screen = new MainMenuScreen(Console.WindowWidth-1, Console.WindowHeight-1);
            screen.Draw();

            Console.ResetColor();
            Console.ReadKey();
        }

        private static void Render(ConsoleMenu menu)
        {
            Console.Clear();
            menu.Draw();
        }
    }
}