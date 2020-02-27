using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleFrontend
{
    public class Window
    {
        public static event EventHandler WindowResized;
        private bool run=true;
        private int width;
        private int height;
        public void Run()
        {
            width = Console.WindowWidth;
            height = Console.WindowHeight;
            var t = new Task(Listen);
            t.Start();
        }

        public void Stop()
        {
            run = false;
        }

        public void Listen()
        {
            while (run)
            {
                if (Console.WindowWidth != width || height != Console.WindowHeight)
                {
                    width  = Console.WindowWidth;
                    height = Console.WindowHeight;
                    WindowResized?.Invoke(this, EventArgs.Empty);
                }
                
                Thread.Sleep(100);
            }
        }
    }
}