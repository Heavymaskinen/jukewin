using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using Juke.Core;

namespace Juke.IO
{
    class SongCatalogue : LoadHandler, LoadListener
    {
        private Library library;
        private LoadListener attachedListener;

        public SongCatalogue(Library library)
        {
            this.library = library;
        }

        public event EventHandler LibraryUpdated;
        public event EventHandler NewLoad;
        public event EventHandler<int> LoadInitiated;
        public event EventHandler<int> LoadProgress;
        public event EventHandler LoadCompleted;
        public event EventHandler<IList<Song>> Completed;

        public void LoadSongs(SongLoader loader)
        {
            UpdateLibrary(loader.LoadSongs(), false);
        }

        public void LoadSongsSync(SongLoader loader)
        {
            NewLoad?.Invoke(this, EventArgs.Empty);
            var songs = loader.LoadSongs();
            LoadInitiated?.Invoke(this, songs.Count);
            UpdateLibrary(songs, false);
            library.InitialiseParts();
            LoadCompleted?.Invoke(this, EventArgs.Empty);
        }

        public Task LoadSongs(IAsyncSongLoader loader)
        {
            return LoadSongs(loader, this);
        }

        public Task LoadSongs(IAsyncSongLoader loader, CancellationToken cancelToken)
        {
            return LoadSongs(loader, this, cancelToken);
        }

        public Task LoadSongs(IAsyncSongLoader loader, LoadListener listener)
        {
            listener.Completed += Listener_Completed;
           return LoadSongs(loader,listener, CancellationToken.None);
        }

        private void Listener_Completed(object sender, IList<Song> list)
        {
            (sender as LoadListener).Completed -= Listener_Completed;
            NotifyCompleted(list);
        }

        public Task LoadSongs(IAsyncSongLoader loader, LoadListener listener, CancellationToken cancelToken)
        {
            attachedListener = listener != this ? listener : null;

            return loader.StartNewLoad(listener, cancelToken);
        }

        public void AddSong(Song song)
        {
            library.AddSong(song);
            //library.InitialiseParts();
            NotifyUpdated();
        }

        public void UpdateSong(SongUpdate songUpdate)
        {
            library.RemoveById(songUpdate.SongSource.ID);
            library.AddSong(songUpdate.ToSong());
           // library.InitialiseParts();
            NotifyUpdated();
        }

        public void DeleteSong(Song song)
        {
            library.DeleteSong(song);
            library.InitialiseParts();
            NotifyUpdated();
        }

        public void DeleteAlbum(string album, LoadListener listener)
        {
            var songsToDelete = library.GetSongsByAlbum(album);
            listener.NotifyNewLoad();
            listener.NotifyLoadInitiated(songsToDelete.Count);
            foreach (var song in songsToDelete)
            {
                library.RemoveById(song.ID);
                listener.NotifyProgress(1);
            }

            library.InitialiseParts();
            listener.NotifyCompleted(new List<Song>());
            NotifyUpdated();

        }

        private void UpdateLibrary(IList<Song> list, bool reload)
        {
            if (reload)
            {
                library.Clear();
            }
            foreach (var song in list)
            {
                library.AddSong(song);
                NotifyProgress(1);
            }

            library.InitialiseParts();
            NotifyUpdated();
        }

        private void NotifyUpdated()
        {
            
            LibraryUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyNewLoad()
        {
            attachedListener?.NotifyNewLoad();
            NewLoad?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyLoadInitiated(int capacity)
        {
            attachedListener?.NotifyLoadInitiated(capacity);
            LoadInitiated?.Invoke(this, capacity);
        }

        public void NotifyProgress(int progressed)
        {
            attachedListener?.NotifyProgress(progressed);
            LoadProgress?.Invoke(this, progressed);
        }

        public void NotifyCompleted(IList<Song> loadedSongs)
        {
            attachedListener?.NotifyCompleted(loadedSongs);
            UpdateLibrary(loadedSongs, true);
            LoadCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
