using System;
using System.IO;
using System.Threading.Tasks;
using ConsoleFrontend.Screens;
using CoreAudioComponent;
using Juke.Control;

namespace ConsoleFrontend
{
    class Program
    {
        static void Main(string[] args)
        {
            var jukeControl = JukeController.Instance;
            jukeControl.Player.RegisterPlayerEngine(new CorePlayerEngine());
            //jukeControl.LoadHandler.LoadSongs(new FakeLoader());
            var gui = new GuiController(jukeControl);

            //Debug();
            gui.Run();
        }

        private static void Debug()
        {
        }

        private static void Render(ConsoleMenu menu)
        {
            Console.Clear();
            menu.Draw();
        }
    }
}