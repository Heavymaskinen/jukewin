using System;

namespace ConsoleFrontend.Overlays
{
    public class PromptOverlay : Overlay
    {
        public event EventHandler<string> TextEntered; 
        private TextBox promptBox;

        public PromptOverlay(Screen parent, string promptMessage) : base(parent)
        {
            AddComponent(new Background {BackgroundColor                   = ConsoleColor.Green});
            var messageBox = new TextBox(promptMessage, '·', '·', 10) {Top = 5, Left = 10};
            promptBox                  = new TextBox("", '*', '*', 20) {Top = 8, Left = 10};
            messageBox.BackgroundColor = BackgroundColor;
            messageBox.ForegroundColor = ForegroundColor;
            promptBox.ForegroundColor  = ForegroundColor;
            promptBox.BackgroundColor  = ConsoleColor.White;
            AddComponent(messageBox);
            AddComponent(promptBox);
            MinHeight = 20;
        }

        protected override void PostDraw()
        {
            Console.SetCursorPosition(promptBox.Left + promptBox.Text.Length + 1, promptBox.Top + 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible   = true;
            Console.Write("");
        }

        protected override void CustomInputHandle(ConsoleKey key)
        {
            var isLetter = key >= ConsoleKey.A && key <= ConsoleKey.Z;
            var isNumber = key >= ConsoleKey.D1 && key <= ConsoleKey.D0;
            var value    = key.ToString().ToLower();
            if (isLetter || isNumber)
            {
                promptBox.TextElement.AppendText(value);
                Invalidate(false);
            }
            else if (key == ConsoleKey.Backspace)
            {
                promptBox.Text = promptBox.Text.Remove(promptBox.Text.Length - 1);
                Invalidate(false);
            }
            else if (key == ConsoleKey.Enter)
            {
                Console.CursorVisible = false;
                Close();
                TextEntered?.Invoke(this, promptBox.Text);
            }
            else
            {
                Invalidate(true);
            }
        }
    }
}