using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Juke.Core;
using JukeControllerCore.IO;

namespace Juke.IO
{
    class SongCatalogue : LoadHandler, SaveHandler
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

        public void SaveSongs(SongWriter writer)
        {
            writer.Write(library.Songs);
        }

        private void AsyncSongLoader_LoadCompleted(object sender, IList<Song> list)
        {
            UpdateLibrary(list);
        }

        private void UpdateLibrary(IList<Song> list)
        {
            Console.WriteLine("Catalogue update "+list.Count);
            foreach (var song in list)
            {
                library.AddSong(song);
            }

            LibraryUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
