using System;
using System.IO;
using Juke.Control;

namespace Juke.Log
{
    public class Logger
    {
        private string filename;
        private static Logger instance;

        public static Logger Start(string filename)
        {
            if (instance == null)
            {
                instance = new Logger(filename);
            }

            return instance;
        }

        private Logger(string filename)
        {
            this.filename = filename;
            Console.WriteLine("Using: " + Directory.GetCurrentDirectory() + filename);
            if (!File.Exists(filename))
            {
                Log("J.U.K.E. log\n===============\n");
            }

            Messenger.LogMessagePosted += Messenger_LogMessagePosted;
        }

        private void Messenger_LogMessagePosted(string message, Messenger.TargetType target)
        {
            LogWithTime(message);
        }

        public void EnableFrontendLog()
        {
            Messenger.FrontendMessagePosted += Messenger_FrontendMessagePosted;
        }

        public void DisableFrontendLog()
        {
            Messenger.FrontendMessagePosted -= Messenger_FrontendMessagePosted;
        }

        private void Messenger_FrontendMessagePosted(string message, Messenger.TargetType target)
        {
            LogWithTime("[Frontend] " + message);
        }

        private void LogWithTime(string message)
        {
            Log(DateTime.Now + "| " + message);
        }

        private void Log(string message)
        {
            lock (filename)
            {
                File.AppendAllText(this.filename, message + "\n");
            }
        }
    }
}