using DataModel;
using Juke.Core;
using Juke.IO;
using JukeControllerCore.IO;
using System;
using System.Collections.Generic;

namespace Juke.Control
{
    public class JukeController : IJukeControl
    {
        private static JukeController instance;
        public static IJukeControl Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new JukeController();
                }

                return instance;
            }
        }

        public static IJukeControl Create()
        {
            return new JukeController();
        }

        private Library library;
        private SongCatalogue catalogue;

        internal JukeController()
        {
            library = new Library();
            catalogue = new SongCatalogue(library);
            Player = new Player();
        }

        public void ClearLibrary()
        {
            library.Clear();
            Player.Queue.Clear();
        }

        public LibraryBrowser Browser
        {
            get { return library; }
        }

        public LoadHandler LoadHandler
        {
            get { return catalogue; }
        }

        public Player Player { get; set; }

        public SaveHandler SaveHandler
        {
            get { return catalogue; }
        }

        public void UpdateSong(SongUpdate edit)
        {
            library.UpdateSong(edit);
        }

        public void DeleteSong(Song song)
        {
            library.DeleteSong(song);
        }

        public void SaveLibrary(SongWriter writer)
        {
            writer.Write(library.Songs);
        }

        public void LoadLibrary(SongLoader loader)
        {
            LoadHandler.LoadSongs(loader);
        }

        private void AsyncSongLoader_LoadCompleted(object sender, IList<Song> loadedSongs)
        {
            foreach (var song in loadedSongs)
            {
                library.AddSong(song);
            }
        }

        public void Dispose()
        {
            Player?.Dispose();
        }
    }
}
