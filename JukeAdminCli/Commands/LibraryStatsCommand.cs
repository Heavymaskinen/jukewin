using System;
using System.IO;
using CoreSongIO;
using Juke.Control;

namespace JukeAdminCli.Commands
{
    public class LibraryStatsCommand: Command
    {
        private SongAccessFactory factory;
        private IJukeControl jukeController;

        public LibraryStatsCommand(IJukeControl jukeController, SongAccessFactory factory)
        {
            this.jukeController = jukeController;
            this.factory = factory;
        }

        public bool Execute(string[] args)
        {
            if (!Validate(args))
            {
                return false;
            }

            try
            {
                var access = new JsonLibraryAccess();
                var loader = new LibraryIO(args[0], access, access);
                jukeController.LoadHandler.LoadSongs(loader);
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
            
            Console.WriteLine("");
            Console.WriteLine("JUKE Library stats: ");
            Console.WriteLine("Artists: " + jukeController.Browser.Artists.Count);
            Console.WriteLine("Albums: " + jukeController.Browser.Albums.Count);
            Console.WriteLine("Songs: " + jukeController.Browser.Songs.Count);
            Console.WriteLine("");

            return true;
        }

        public Documentation GetDocumentation()
        {
            return new Documentation("stats", "Print stats for a given library", new string[] { "Library json-filename" });
        }

        public bool Validate(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Missing parameter: Library file");
                return false;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("File doesn't exist");
                return false;
            }

            return true;
        }
    }
}
