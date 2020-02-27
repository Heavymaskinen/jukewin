namespace ConsoleFrontend
{
    public interface TextElement : IDrawable
    {
        public string Text { get; }
        public void AppendText(string txt);
        public void ClearText();
        public void SetText(string txt);
    }
}