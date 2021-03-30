using System;
using System.Collections.Generic;
using DataModel;

namespace Juke.Core
{
    public interface IPlayer
    {
        public event EventHandler<Song> SongPlayed;
        Song NowPlaying { get; }
        IList<Song> EnqueuedSongs { get; }
        void RegisterPlayerEngine(PlayerEngine engine);
        void PlayAlbum(string selectedAlbum);
        void PlaySong(Song song);
        void Dispose();
        void PlayNext();
        void Stop();
        void PlayRandom();
        void Skip();
    }
}