using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Juke.IO;
using DataModel;

namespace Juke.Control.Tests
{
    public class FakeSongIO : SongLoader, SongWriter
    {
        private IList<Song> songsToLoad;

        private IList<Song> writtenSongs;

        public FakeSongIO():this(new List<Song>())
        {
        }

        public FakeSongIO(IList<Song> songsToLoad)
        {
            this.songsToLoad = songsToLoad;
            writtenSongs = new List<Song>();
        }

        public IList<Song> WrittenSongs
        {
            get
            {
                return writtenSongs;
            }
          
        }

        public IList<Song> LoadSongs()
        {
            return songsToLoad;
        }

        public void Write(IList<Song> songs)
        {
            writtenSongs = songs;
        }
    }
}
