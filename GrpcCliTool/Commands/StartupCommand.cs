using System.Diagnostics;
using System.IO;

namespace GrpcClient.Commands
{
    public class StartupCommand : Command
    {
        public string Name => "start";
        public string Description => "Start the JUKE service - optionally provide a path";

        public void Execute(JukeClient client, CommandOutput output, string[] arguments)
        {
            var processes = Process.GetProcessesByName("GrpcJukeServer");
            if (processes.Length > 0)
            {
                output.WriteError("Service already started");
                client.StreamOutput();
                return;
            }

            string path = "./GrpcJukeServer";
            if (arguments.Length > 0)
            {
                path = arguments[0];
            }

            output.WriteMessage("Starting from " + path);

            if (!File.Exists(path))
            {
                output.WriteError("File doesn't exist.");
                return;
            }

            var result = client.Startup(path, arguments.Length > 1);
            if (result)
            {
                output.WriteMessage("Service started");
                client.StreamOutput();
            }
            else
            {
                output.WriteError("Service NOT started");
            }
        }
    }
}