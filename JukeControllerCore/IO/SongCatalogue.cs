using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void LoadSongs(SongLoader loader)
        {
            UpdateLibrary(loader.LoadSongs());
        }

        public void LoadSongs(AsyncSongLoader loader)
        {
            loader.BeginLoading();
        }

        private void AsyncSongLoader_LoadCompleted(object sender, IList<Song> list)
        {
            UpdateLibrary(list);
        }

        private void UpdateLibrary(IList<Song> list)
        {
            foreach (var song in list)
            {
                library.AddSong(song);
            }

            LibraryUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
