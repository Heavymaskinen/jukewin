using Juke.Core;
using Juke.IO;

namespace Juke.Control
{
    public interface IJukeController
    {
        LibraryBrowser Browser { get; }
        LoadHandler LoadHandler { get; }
        IPlayer Player { get; set; }
        void SaveLibrary(SongWriter writer);
        void LoadLibrarySync(SongLoader loader);
        void Dispose();
    }
}