namespace Juke.Core
{
    public interface TagReader
    {
        string Title { get; }
        string Album { get; }
        string Artist { get; }
        string TrackNo { get; }
    }
}