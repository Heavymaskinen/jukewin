namespace GrpcClient.Commands
{
    public class PlayRandomCommand:Command
    {
        public string Name => "random";
        public string Description => "Play a random song";
        public void Execute(JukeClient client, CommandOutput output, string[] arguments)
        {
            var info = client.PlayRandom();
            output.WriteMessage("Added random");
        }
    }
}