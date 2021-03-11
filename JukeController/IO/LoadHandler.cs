using DataModel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Juke.IO
{
    public interface LoadHandler
    {
        event EventHandler NewLoad;
        event EventHandler<int> LoadInitiated;
        event EventHandler<int> LoadProgress;
        event EventHandler LoadCompleted;
        event EventHandler LibraryUpdated;

        void LoadSongs(SongLoader loader);
        Task LoadSongs(IAsyncSongLoader loader);
        Task LoadSongs(IAsyncSongLoader loader, LoadListener listener);
        void LoadSongsSync(SongLoader loader);
        Task LoadSongs(IAsyncSongLoader loader, CancellationToken cancelToken);
    
        void UpdateSong(SongUpdate songUpdate);
        void DeleteSong(Song song);
        void DeleteAlbum(string albumName, LoadListener progressTracker);
        void DeleteArtist(string artist, LoadListener progressTracker);
        void RenameArtist(SongUpdate songUpdate);
        void RenameAlbum(SongUpdate songUpdate);
        void DeleteSongs(IList<Song> selectedSongs, LoadListener loadListener);
    }
}