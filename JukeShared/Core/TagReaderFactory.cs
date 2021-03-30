namespace Juke.Core
{
    public interface TagReaderFactory
    {
        TagReader Create(string filename);
        TagReaderFactory BackupFactory { get; set; }
    }
}