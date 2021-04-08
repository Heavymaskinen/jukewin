namespace GrpcClient.Commands
{
    public class LoadLibraryCommand: Command
    {
        public string Name => "load";
        public string Description => "Load the library from a specific XML-file";

        public void Execute(JukeClient client, CommandOutput output, string[] arguments)
        {
            if (arguments.Length < 1)
            {
                output.WriteError("Missing argument filename");
                return;
            }

            var response = client.LoadLibrary(arguments[0]);
            if (response)
            {
                output.WriteMessage("Command completed.");
            }
            else
            {
                output.WriteError("Command failed");
            }
        }
    }
}