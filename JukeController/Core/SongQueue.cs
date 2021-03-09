using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Core
{
    public class SongQueue
    {
        private Queue<Song> queue;
        private LibraryBrowser browser;
        private Randomizer randomizer;
        public Song Next
        {
            get
            {
                if (Count > 0)
                {
                    return queue.First();
                }

                return null;
            }
        }

        public Song Random => randomizer.Next();

        public List<Song> Songs
        {
            get { return queue.ToList(); }
        }

        public int Count
        {
            get { return queue.Count; }
        }

        public SongQueue(LibraryBrowser browser)
        {
            queue = new Queue<Song>();
            this.browser = browser;
            randomizer = new Randomizer(browser);
        }

        public void Enqueue(Song song)
        {
            if (!queue.Contains(song))
            {
                queue.Enqueue(song);
            }
        }

        public void EnqueueAlbum(string albumName)
        {
            var songs =browser.GetSongsByAlbum(albumName);
            foreach ( var s in songs)
            {
                Enqueue(s);
            }
        }

        public Song Dequeue()
        {
            if (queue.Count > 0)
            {
                var song = queue.Dequeue();
                randomizer.Add(song);
                return song;
            }

            if (browser.Songs.Count <= 1)
            {
                return null;
            }

            return randomizer.Next();
        }

        internal void Clear()
        {
            queue.Clear();
        }
    }
}
