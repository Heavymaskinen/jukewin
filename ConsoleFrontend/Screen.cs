using System;

namespace ConsoleFrontend
{
    public abstract class Screen
    {
        public static event EventHandler Invalidated;
        public static event EventHandler<ScreenName> ScreenChange;

        public int Width { get; set; }
        public int MinHeight { get; set; }

        public void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        protected void ChangeScreen(ScreenName name)
        {
            ScreenChange?.Invoke(this, name);
        }

        public abstract void Draw();

        public abstract void UpdateInput(ConsoleKey key);
    }
}