namespace GrpcClient.Commands
{
    public class AddSongsCommand:Command
    {
        public string Name => "add";
        public string Description => "Add songs from specified folder to library";
        public void Execute(JukeClient client, CommandOutput output, string[] arguments)
        {
            client.AddSongs(arguments[0]);
        }
    }
}