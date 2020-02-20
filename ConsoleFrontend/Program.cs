using System;
using System.IO;
using System.Threading.Tasks;
using Juke.Control;

namespace ConsoleFrontend
{
    class Program
    {
        static void Main(string[] args)
        {
            var gui = new GuiController(JukeController.Instance);
            gui.Run();
        }

        private static void Render(ConsoleMenu menu)
        {
            Console.Clear();
            menu.Draw();
        }
    }
}