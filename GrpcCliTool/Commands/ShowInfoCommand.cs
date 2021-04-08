using System;

namespace GrpcClient.Commands
{
    public class ShowInfoCommand: Command
    {
        public string Name => "info";
        public string Description => "Get info from J.U.K.E.";
        public void Execute(JukeClient client, CommandOutput output, string[] arguments)
        {
            var info = client.GetInfo();
            Console.WriteLine("J.U.K.E. Info:");
            foreach (var key in info.Keys)
            {
                Console.WriteLine(key+": "+info[key]);
            }
            
            Console.WriteLine("------------------");
        }
    }
}