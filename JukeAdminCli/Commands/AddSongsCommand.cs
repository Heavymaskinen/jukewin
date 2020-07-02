using Juke.Control;

namespace JukeAdminCli.Commands
{
    public class AddSongsCommand : Command
    {
        private IJukeControl jukeControl;
        private AbstractSongAccessFactory factory;

        public AddSongsCommand(IJukeControl jukeControl, AbstractSongAccessFactory factory)
        {
            this.jukeControl = jukeControl;
            this.factory = factory;
        }

        public bool Execute(string[] args)
        {
            Validate(args);
            var loader = factory.CreateLoader("*.mp3", args[0]);
            jukeControl.LoadHandler.LoadSongs(loader);

            var writer = factory.CreateWriter(args[1]);
            jukeControl.SaveHandler.SaveSongs(writer);
            return true;
        }

        public Documentation GetDocumentation()
        {
            return new Documentation("add", "Add songs from folder to a specified library file", new string[] {
                "Target folder", "Library file"
            });
        }

        private static void Validate(string[] args)
        {
            if (args.Length < 1)
            {
                throw new System.Exception("Missing first parameter: Target folder");
            }

            if (args.Length < 2)
            {
                throw new System.Exception("Missing second parameter: Library file");
            }
        }

        
    }
}
