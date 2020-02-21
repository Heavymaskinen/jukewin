using System.Collections.Generic;
using DataModel;
using Juke.IO;

namespace ConsoleFrontend
{
    public class FakeLoader : SongLoader
    {
        public IList<Song> LoadSongs()
        {
            return new List<Song>()
            {
                new Song("Super singer", "Super album", "Super song"),
                new Song("Super singer", "Super album", "Super song v2"),
                new Song("Super singer", "Super album", "Best song")
            };
        }
    }
}