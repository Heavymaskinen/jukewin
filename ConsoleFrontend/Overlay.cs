using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConsoleFrontend
{
    public abstract class Overlay : IDrawable
    {
        public static event EventHandler Opened;
        public static event EventHandler Closed;

        public int Top { get; set; }
        public int Left { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        private IList<IDrawable> components = new Collection<IDrawable>();

        protected void AddComponent(IDrawable drawable)
        {
            drawable.Left += Left;
            drawable.Top  += Top;
            components.Add(drawable);
        }

        protected void Open()
        {
            Opened?.Invoke(this, EventArgs.Empty);
        }

        protected void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void Draw()
        {
            foreach (var comp in components)
            {
                Console.ForegroundColor = comp.ForegroundColor;
                Console.BackgroundColor = comp.BackgroundColor;
                Console.SetCursorPosition(comp.Left, comp.Top);
                comp.Draw();
            }

            Console.ResetColor();
        }

        public abstract void UpdateInput(ConsoleKey key);
    }
}