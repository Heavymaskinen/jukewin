using System;
using Juke.Control;

namespace Juke.Log2
{
    public class Logger
    {
        public static void ConsoleLog()
        {
            Messenger.LogMessagePosted += Messenger_LogMessagePosted;
        }

        private static void Messenger_LogMessagePosted(string message, Messenger.TargetType target)
        {
            Console.WriteLine(message);
        }
    }
}