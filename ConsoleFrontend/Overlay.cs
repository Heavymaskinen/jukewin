using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConsoleFrontend
{
    public abstract class Overlay : IDrawable
    {
        public event EventHandler Opened;
        public event EventHandler Closed;

        public int Top { get; set; }
        public int Left { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public int Width { get; set; }
        public int MinHeight { get; set; }

        private IList<IDrawable> components = new Collection<IDrawable>();

        private Screen parent;

        protected Overlay(Screen parent)
        {
            this.parent = parent;
            parent.AssignOverlay(this);
        }

        protected void AddComponent(IDrawable drawable)
        {
            drawable.Left += Left;
            drawable.Top  += Top;
            components.Add(drawable);
        }

        public void Open()
        {
            Opened?.Invoke(this, EventArgs.Empty);
        }

        protected void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public void Draw()
        {
            Console.Clear();
            foreach (var comp in components)
            {
                Console.ForegroundColor = comp.ForegroundColor;
                Console.BackgroundColor = comp.BackgroundColor;
                Console.SetCursorPosition(comp.Left, comp.Top);
                comp.Draw();
            }

            Console.ResetColor();
            PostDraw();
        }

        protected virtual void PostDraw()
        {
            
        }

        public void ReducedDraw()
        {
            foreach (var comp in components)
            {
                Console.ForegroundColor = comp.ForegroundColor;
                Console.BackgroundColor = comp.BackgroundColor;
                Console.SetCursorPosition(comp.Left, comp.Top);
                comp.ReducedDraw();
            }

            Console.ResetColor();
        }

        public void UpdateInput(ConsoleKey key)
        {
            if (key == ConsoleKey.Escape)
            {
                Close();
            }
            else
            {
                CustomInputHandle(key);
            }
        }

        protected void Invalidate(bool fullRedraw)
        {
            parent.Invalidate(fullRedraw);
        }

        protected abstract void CustomInputHandle(ConsoleKey key);
    }
}