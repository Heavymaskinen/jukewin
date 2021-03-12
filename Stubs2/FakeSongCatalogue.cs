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
        public static List<Song> CreateSongs(int artistMax, int albumMax, int songmax)
        {
            var songList = new List<Song>();
            for (var artist = 1; artist <= artistMax; artist++)
            {
                for (var album = 1; album <= albumMax; album++)
                {
                    for (var song = 1; song <= songmax; song++)
                    {
                        songList.Add(new Song("artist" + artist, "album" + album, "song" + song,
                            artist + song - 1 + "", artist + "/" + album + "/" + song));
                    }
                }
            }

            return songList;
        }

        private IList<Song> songsToLoad;

        public FakeSongCatalogue() : this(new List<Song>())
        {
        }

        public FakeSongCatalogue(IList<Song> songsToLoad)
        {
            this.songsToLoad = songsToLoad;
            WrittenSongs = new List<Song>();
        }

        public IList<Song> WrittenSongs { get; private set; }

        public IList<Song> LoadSongs()
        {
            return songsToLoad;
        }

        public void Write(IList<Song> songs)
        {
            WrittenSongs = songs;
        }
    }
}