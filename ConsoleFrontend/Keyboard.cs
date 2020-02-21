using System;
using System.Threading.Tasks;

namespace ConsoleFrontend
{
    public class Keyboard
    {
        public event EventHandler<ConsoleKey> KeyPressed;
        private bool run = true;

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