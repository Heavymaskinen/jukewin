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

        public List<Song> Songs
        {
            get { return queue.ToList(); }
        }

        public int Count
        {
            get { return queue.Count; }
        }

        public SongQueue()
        {
            queue = new Queue<Song>();
        }

        public void Enqueue(Song song)
        {
            if (!queue.Contains(song))
            {
                queue.Enqueue(song);
            }
            
        }

        public Song Dequeue()
        {
            if (queue.Count >0)
            {
                return queue.Dequeue();
            }

            return null;
        }

        internal void Clear()
        {
            queue.Clear();
        }
    }
}
