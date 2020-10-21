using System;

namespace JukeAdminCli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args);
            new Interpreter().Run(args);
        }
    }
}
