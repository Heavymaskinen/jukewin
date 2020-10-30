using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grpc.Core;

namespace GrpcClient
{
    public class CommandInterpreter : CommandOutput
    {
        private JukeClient client;

        private List<Command> commands = new List<Command>();

        public CommandInterpreter() : this(new GrpcJukeClient())
        {
        }

        public CommandInterpreter(JukeClient client)
        {
            this.client = client;
            var currentAssembly = Assembly.GetAssembly(GetType());
            var types           = currentAssembly.GetTypes();
            foreach (var type in types)
            {
                if (!type.IsAbstract && type.IsClass
                                     && type.GetInterfaces().Contains(typeof(Command)))
                {
                    var instance = (Command)  Activator.CreateInstance(type);;
                    commands.Add(instance);
                }
            }
        }

        public IEnumerable<string> GetCommandNames()
        {
            return commands.Select(c => c.Name);
        }

        public void Run(string[] args)
        {
            foreach (var command in commands)
            {
                if (command.Name == args[0])
                {
                    RunCommand(args, command);
                    return;
                }
            }

            WriteError("Command " + args[0] + " not found!");
        }

        private void RunCommand(string[] args, Command command)
        {
            try
            {
                command.Execute(client, this, args.Length > 1 ? new[] {args[1]} : new string[0]);
            }
            catch (RpcException x)
            {
                Console.Error.WriteLine("Failed to connect to service: "+x.Message);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Failed running command ("+command.Name+"): "+e.Message);
            }
        }

        public void WriteMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        public void WriteError(string msg)
        {
            Console.Error.WriteLine(msg);
        }
    }
}