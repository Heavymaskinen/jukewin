namespace GrpcClient.Commands
{
    public class SearchCommand : Command
    {
        public string Name => "search";
        public string Description => "Search for songs";
        public void Execute(JukeClient client, CommandOutput output, string[] arguments)
        {
            if (arguments.Length < 1)
            {
                output.WriteError("Missing query argument");
                return;
            }
            
            var response = client.Search(arguments[0]);
            output.WriteMessage("Found:");
            foreach (var info in response)
            {
                output.WriteMessage(info);
            }
            output.WriteMessage("--------------");
        }
    }
}