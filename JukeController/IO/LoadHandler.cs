using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        void AddSong(Song song);
        void UpdateSong(SongUpdate songUpdate);
        void DeleteSong(Song song);
    }
}
