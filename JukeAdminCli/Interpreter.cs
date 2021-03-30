using System;
using System.Collections.Generic;
using Juke.Control;
using JukeAdminCli.Commands;

namespace JukeAdminCli
{
    public class Interpreter
    {
        private static List<Command> commands;

        public Interpreter()
        {
            var jukeControl = JukeController.Instance;
            var factory = new SongAccessFactory();
            commands = new List<Command>()
            {
                new AddSongsCommand(jukeControl, factory),
                new LibraryStatsCommand(jukeControl, factory)
            };
        }

        public void Run(string[] input)
        {
            if (input.Length < 1)
            {
                Console.WriteLine("Provide command name.");
                Console.WriteLine("Available commands:");
                foreach (var c in commands)
                {
                    Console.WriteLine(c.GetDocumentation());
                }
                return;
            }

            foreach (var c in commands)
            {
                if (c.GetDocumentation().Name == input[0])
                {
                    var list = new List<string>(input);
                    list.RemoveAt(0);
                    var trimmedInput = list.ToArray();

                    try
                    {
                        c.Execute(trimmedInput);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                    }

                    Console.WriteLine("OK");
                    return;
                }
            }

            Console.WriteLine("Command not found with params: " + input);

        }
    }
}
