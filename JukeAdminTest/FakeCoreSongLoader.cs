using System.Collections.Generic;
using DataModel;
using Juke.IO;

namespace JukeAdminTest
{
    public class FakeSongLoader : SongLoader
    {
        public string UsedPath;
        public IList<Song> SongsToLoad { get; set; }

        public FakeSongLoader(string path)
        {
            UsedPath = path;
            SongsToLoad = new List<Song>();
        }

        public IList<Song> LoadSongs()
        {
            return SongsToLoad;
        }
    }
}
