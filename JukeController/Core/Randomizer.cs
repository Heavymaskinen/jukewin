using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Core
{
    internal class Randomizer
    {
        private Queue<Song> usedSongs;
        private LibraryBrowser library;

        internal Randomizer(LibraryBrowser library)
        {
            this.library = library;
            usedSongs = new Queue<Song>(10);
        }

        internal Song Next()
        {
            var rand = new Random(Environment.TickCount);
            if (usedSongs.Count == library.Songs.Count)
            {
                return null;
            }

            Song nextSong = usedSongs.Count > 0? usedSongs.Peek() : null;
            while (nextSong == null || usedSongs.Contains(nextSong))
            {
                var ind = rand.Next(0, library.Songs.Count);
                nextSong = library.Songs[ind];
            }

            usedSongs.Enqueue(nextSong);
            if (usedSongs.Count >= 10)
            {
                usedSongs.Dequeue();
            }

            return nextSong;
        }

        internal void Add(Song song)
        {
            usedSongs.Enqueue(song);
        }
    }
}
