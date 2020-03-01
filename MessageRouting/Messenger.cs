using System;

namespace MessageRouting
{
    public class Messenger
    {
        public static event EventHandler<string> MessagePosted;

        public static void Post(string message)
        {
            MessagePosted?.Invoke(null, message);
        }
    }
}