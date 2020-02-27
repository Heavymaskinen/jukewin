using System;

namespace ConsoleFrontend
{
    public abstract class Screen
    {
        public static event EventHandler<bool> Invalidated;
        public static event EventHandler<ScreenName> ScreenChange;

        public int Width { get; set; }
        public int MinHeight { get; set; }

        public bool Visible { get; set; }
        
        public bool HasOverlay { get; private set; }

        private Overlay dialog;
        

        protected Screen()
        {
            Visible = false;
        }

        public void Invalidate(bool fullRepaint)
        {
            GuiController.LogList.Add("Invalidated: "+GetType());
            Invalidated?.Invoke(this, fullRepaint);
        }

        public virtual void Resized(int newWidth, int newHeight)
        {
            Width     = newWidth;
            MinHeight = newHeight;
        }

        protected void ChangeScreen(string name)
        {
            var result = Enum.TryParse(typeof(ScreenName), name, true, out var parsed);
            if (result)
            {
                ChangeScreen((ScreenName) parsed);
            }
        }

        protected void ChangeScreen(ScreenName name)
        {
            ScreenChange?.Invoke(this, name);
        }

        public void Redraw()
        {
            if (!Visible) return;

            Console.Clear();
            if (dialog != null)
            {
                GuiController.LogList.Add("Full draw dialog "+dialog);
                dialog.Draw();
            }
            else
            {
                CustomRedraw();
            }
        }

        public void Draw()
        {
            if (!Visible) return;

            if (dialog != null)
            {
                GuiController.LogList.Add("Small draw dialog "+dialog);
                dialog.Draw();
            }
            else
            {
                CustomDraw();
            }
        }

        public void AssignOverlay(Overlay overlay)
        {
            overlay.Opened += OverlayOnOpened;
            overlay.Closed += OverlayOnClosed;
        }

        protected abstract void CustomRedraw();
        protected abstract void CustomDraw();

        public void UpdateInput(ConsoleKey key)
        {
            if (dialog != null)
            {
                GuiController.LogList.Add("Dialog key input: "+key+", "+dialog);
                dialog.UpdateInput(key);
            }
            else
            {
                GuiController.LogList.Add(" Key input: "+key+", "+GetType());
                CustomUpdateInput(key);
            }
        }

        protected abstract void CustomUpdateInput(ConsoleKey key);

        private void OverlayOnOpened(object sender, EventArgs e)
        {
            GuiController.LogList.Add("Dialog opened, "+sender);
            dialog = (Overlay) sender;
            HasOverlay = true;
            Invalidate(true);
        }

        private void OverlayOnClosed(object sender, EventArgs e)
        {
            GuiController.LogList.Add("Dialog closed, "+sender);
            dialog = null;
            HasOverlay = false;
            Invalidate(true);
        }
    }
}