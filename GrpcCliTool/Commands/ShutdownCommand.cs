namespace GrpcClient.Commands
{
    public class ShutdownCommand : Command
    {
        public string Name => "stop";
        public string Description => "Stop the JUKE service";
        public void Execute(JukeClient client, CommandOutput output, string[] arguments)
        {
            client.Shutdown();
        }
    }
}