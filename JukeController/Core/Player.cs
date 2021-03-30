using DataModel;
using System;
using System.Collections.Generic;
using Juke.Control;

namespace Juke.Core
{
    public class Player : IPlayer
    {
        public event EventHandler<Song> SongPlayed;

        private PlayerEngine engine;

        public SongQueue Queue { get; }


        public Player(LibraryBrowser browser)
        {
            Queue = new SongQueue(browser);
        }

        public Song NowPlaying { get; private set; }
        
        public IList<Song> EnqueuedSongs => Queue.Songs;

        public void RegisterPlayerEngine(PlayerEngine engine)
        {
            this.engine = engine;
            engine.SongFinished += Engine_SongFinished;
        }

        public void PlayAlbum(string selectedAlbum)
        {
            Queue.EnqueueAlbum(selectedAlbum);
            if (NowPlaying == null)
            {
                PlayNext();
            }
        }

        public void PlaySong(Song song)
        {
            if (song == null)
            {
                Messenger.Log("PlaySong called with null");
                return;
            }

            if (NowPlaying == null)
            {
                NowPlaying = song;
                engine.Play(song);
                Messenger.Log("Playing " + song);
            }
            else if (!song.Equals(NowPlaying))
            {
                Queue.Enqueue(song);
                Messenger.Log("Enqueued " + song);
            }

            SongPlayed?.Invoke(this, song);
        }

        public void Dispose()
        {
            engine?.Dispose();
        }

        private void Engine_SongFinished(object sender, EventArgs e)
        {
            NowPlaying = null;
            PlayNext();
        }

        public void PlayNext()
        {
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

        public void Skip()
        {
            engine.Stop();
        }
    }
}