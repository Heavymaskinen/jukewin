using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Control;
using MessageRouting;

namespace Juke.Core
{
    public class Player : IDisposable
    {
        public static event EventHandler<Song> SongPlayed;

        private PlayerEngine engine;

        public SongQueue Queue { get; private set; }
        

        public Player()
        {
            Queue = new SongQueue();
        }
        

        public Song NowPlaying { get; private set; }

        public void RegisterPlayerEngine(PlayerEngine engine)
        {
            this.engine = engine;
            engine.SongFinished += Engine_SongFinished;
        }

        public void PlaySong(Song song)
        {
            if (song == null && JukeController.Instance.Browser.Songs.Count > 0 )
            {
                var rand = new Random();
                var newIndex =rand.Next(0, JukeController.Instance.Browser.Songs.Count);
                song = JukeController.Instance.Browser.Songs[newIndex];
            }

            if (engine == null)
            {
                Messenger.Post("No engine registered! Unable to play");
                return;
            }

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

        private void Engine_SongFinished(object sender, EventArgs e)
        {
            NowPlaying = null;
            PlaySong(Queue.Dequeue());
        }

        public void Stop()
        {
            engine.Stop();
        }

        public void Dispose()
        {
            engine?.Dispose();
        }
    }
}
