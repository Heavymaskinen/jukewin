using System;

namespace ConsoleFrontend
{
    public class TextBox : IDrawable
    {
        private IDrawable border;
        private TextElement label;
        private IDrawable background;

        public int Top { get; set; }
        public int Left { get; set; }

        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public int Width { get; set; }
        public int MinHeight { get; set; }

        public TextElement TextElement => label;

        public string Text
        {
            get => label.Text;
            set => label.SetText(value);
        }

        public TextBox(string text, char verticalBorder, char horizontalBorder, int width)
        {
            label           = new Label(text);
            border          = new Border(horizontalBorder, verticalBorder, width, 3);
            background      = new Background();
            Width           = Math.Max(width, text.Length + 2); //Text + each vertical border
            MinHeight       = 3;
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
        }

        public TextBox(TextElement textElement, char verticalBorder, char horizontalBorder, int width)
        {
            label           = textElement;
            border          = new Border(horizontalBorder, verticalBorder, width, 3);
            background      = new Background();
            Width           = Math.Max(width, textElement.Width + 2); //Text + each vertical border
            MinHeight       = 2 + textElement.MinHeight;
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
        }

        public void Draw()
        {
            Width                 = Math.Max(Width, label.Width + 2); //Text + each vertical border
            MinHeight             = 2 + label.MinHeight;
            label.Top             = Top + 1;
            label.Left            = Left + 1;
            label.ForegroundColor = ForegroundColor;
            label.BackgroundColor = BackgroundColor;

            background.Left            = Left;
            background.Top             = Top;
            background.BackgroundColor = BackgroundColor;
            background.Width           = Width;
            background.MinHeight       = MinHeight;

            border.Top             = Top;
            border.Left            = Left;
            border.BackgroundColor = BackgroundColor;
            border.ForegroundColor = ForegroundColor;
            border.Width           = Width;
            border.MinHeight       = MinHeight;

            background.Draw();
            label.Draw();
            border.Draw();
        }
    }
}