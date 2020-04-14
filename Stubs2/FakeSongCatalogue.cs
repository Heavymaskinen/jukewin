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
    public class FakeSongCatalogue : SongLoader, SongWriter
    {
        private IList<Song> songsToLoad;

        private IList<Song> writtenSongs;

        public FakeSongCatalogue():this(new List<Song>())
        {
        }

        public FakeSongCatalogue(IList<Song> songsToLoad)
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

        public override IList<Song> LoadSongs()
        {
            return songsToLoad;
        }

        public void Write(IList<Song> songs)
        {
            writtenSongs = songs;
        }
    }
}
