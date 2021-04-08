namespace GrpcClient.Commands
{
    public class PlayCommand :Command
    {
        public string Name => "play";
        public string Description => "Play a specific song";
        public void Execute(JukeClient client, CommandOutput output, string[] arguments)
        {
            if (arguments.Length < 1)
            {
                output.WriteError("Missing song title param");
                return;
            }

            output.WriteMessage("Enqueueing "+arguments[0]);
            client.Play(arguments[0]);
        }
    }
}