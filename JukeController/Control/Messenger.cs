using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Control
{
    public class Messenger
    {
        public enum TargetType
        {
            Frontend,
            Log
        }

        public delegate void MessageHandler(string message, TargetType target);

        public static event MessageHandler LogMessagePosted;
        public static event MessageHandler FrontendMessagePosted;

        public static void Log(string message)
        {
            PostMessage(message, TargetType.Log);
        }

        public static void PostMessage(string message, TargetType target)
        {
            if (target == TargetType.Frontend)
            {
                FrontendMessagePosted?.Invoke(message, target);
            }
            else if (target == TargetType.Log)
            {
                LogMessagePosted?.Invoke(message, target);
            }
        }

        public static void Post(string message)
        {
            PostMessage(message, TargetType.Frontend);
        }
    }
}