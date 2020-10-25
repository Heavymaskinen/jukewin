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
    class SongCatalogue : LoadHandler
    {
        private Library library;

        public SongCatalogue(Library library)
        {
            this.library = library;
            AsyncSongLoader.LoadCompleted += AsyncSongLoader_LoadCompleted;
        }

        public event EventHandler LibraryUpdated;
        public event EventHandler NewLoad;
        public event EventHandler<int> LoadInitiated;
        public event EventHandler<int> LoadProgress;
        public event EventHandler LoadCompleted;

        public void LoadSongs(SongLoader loader)
        {
            Task.Run(() => UpdateLibrary(loader.LoadSongs(), false));
        }

        public void LoadSongsSync(SongLoader loader)
        {
            NewLoad?.Invoke(this, EventArgs.Empty);
            var songs = loader.LoadSongs();
            LoadInitiated?.Invoke(this, songs.Count);
            UpdateLibrary(songs, true);
            LoadCompleted?.Invoke(this, EventArgs.Empty);
        }

        public void LoadSongs(AsyncSongLoader loader)
        {
            loader.StartNewLoad();
            //loader.BeginLoading();
        }

        public void AddSong(Song song)
        {
            library.AddSong(song);
            library.InitialiseParts();
            NotifyUpdated();
        }

        public void UpdateSong(SongUpdate songUpdate)
        {
            library.RemoveById(songUpdate.SongSource.ID);
            library.AddSong(songUpdate.ToSong());
            library.InitialiseParts();
            NotifyUpdated();
        }

        public void DeleteSong(Song song)
        {
            library.DeleteSong(song);
            NotifyUpdated();
        }

        private void AsyncSongLoader_LoadCompleted(object sender, IList<Song> list)
        {
            UpdateLibrary(list, false);
        }

        private void UpdateLibrary(IList<Song> list, bool reload)
        {
            foreach (var song in list)
            {
                library.AddSong(song, reload);
                LoadProgress?.Invoke(this, 1);
            }

            library.InitialiseParts();
            NotifyUpdated();
        }

        private void NotifyUpdated()
        {
            LibraryUpdated?.Invoke(this, EventArgs.Empty);
        }

    }
}
