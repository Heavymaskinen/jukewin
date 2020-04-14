using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Core
{
    public class Player
    {
        public static event EventHandler<Song> SongPlayed;

        private PlayerEngine engine;

        public SongQueue Queue { get; private set; }
        

        public Player(LibraryBrowser browser)
        {
            Queue = new SongQueue(browser);
        }

        public Player(PlayerEngine engine)
        {
            RegisterPlayerEngine(engine);
        }

        public Song NowPlaying { get; private set; }

        public void RegisterPlayerEngine(PlayerEngine engine)
        {
            this.engine = engine;
            engine.SongFinished += Engine_SongFinished;
        }

        public void PlaySong(Song song)
        {
            if (NowPlaying == null)
            {
                NowPlaying = song;
                engine.Play(song);
            }
            else if (!song.Equals(NowPlaying))
            {
                Queue.Enqueue(song);
            }

            SongPlayed?.Invoke(this, song);
        }

        internal void Dispose()
        {
            engine?.Dispose();
        }

        private void Engine_SongFinished(object sender, EventArgs e)
        {
            NowPlaying = null;
            PlaySong(Queue.Dequeue());
        }

        public void Stop()
        {
            engine.Stop();
        }

        public void PlayRandom()
        {
            PlaySong(Queue.Random);
        }
    }
}
