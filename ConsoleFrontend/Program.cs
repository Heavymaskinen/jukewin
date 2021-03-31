using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using ConsoleFrontend.Screens;
using CoreAudioComponent;
using CoreSongIO;
using DataModel;
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
            //Debug();
            var gui = new GuiController(jukeControl);

            gui.Run();
        }

        private static void Debug()
        {
            var list = new PagedMenu("Hejhej",
                new Collection<ConsoleMenuItem>
                {
                    new ConsoleMenuItem {ID = "Flot", Label = "Det er flot!"},
                    new ConsoleMenuItem {ID = "Flot", Label = "Det er SÅ flot!"},
                    new ConsoleMenuItem {ID = "Flot", Label = "Det er MEGA flot!"},
                });
            list.Draw();
            Console.ReadKey();
        }

        private static void Render(ConsoleMenu menu)
        {
            Console.Clear();
            menu.Draw();
        }
    }
}