using System;
using Juke.Control;

namespace ConsoleFrontend
{
    public class MessageBox : IDrawable
    {
        private TextBox textBox;
        private MultiLineText multiLineText;

        public int Top { get; set; }
        public int Left { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public int Width
        {
            get => multiLineText.Width;
            set => multiLineText.Width = value;
        }

        public int MinHeight { get; set; }

        public MessageBox(int lines)
        {
            multiLineText           =  new MultiLineText {MaxLines = lines};
            textBox                 =  new TextBox(multiLineText, '|', '=', 12);
            Messenger.FrontendMessagePosted += MessengerOnMessagePosted;
        }

        private void MessengerOnMessagePosted(string txt, Messenger.TargetType type)
        {
            multiLineText.AppendText(txt);
        }

        public void Draw()
        {
            textBox.BackgroundColor = BackgroundColor;
            textBox.ForegroundColor = ForegroundColor;
            textBox.Top = Top;
            textBox.Left = Left;
            textBox.Draw();
        }
    }
}