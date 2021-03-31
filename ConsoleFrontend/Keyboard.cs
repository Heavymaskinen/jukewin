using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleFrontend
{
    public class Keyboard
    {
        public static event EventHandler<ConsoleKey> KeyPressed;
        private bool run = true;

        public static void Run()
        {
            Task t = new Task(() =>
            {
                var key = new Keyboard();
                key.Listen();
            });
            t.Start();
        }

        public async void Listen()
        {
            while (run)
            {
                var key = await ReadKeys();
                KeyPressed?.Invoke(this, key);
            }
        }

        public void Stop()
        {
            run = false;
        }

        private Task<ConsoleKey> ReadKeys()
        {
            var task = new Task<ConsoleKey>(() => Console.ReadKey().Key);
            task.Start();
            return task;
        }
    }
}