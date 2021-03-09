using DataModel;
using Juke.Core;
using Juke.IO;
using System.Collections.Generic;

namespace Juke.Control
{
    public class JukeController
    {
        private static JukeController instance;
        public static JukeController Instance
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

        public static void Reset()
        {
            instance = new JukeController();
        }

        public static JukeController Create()
        {
            return new JukeController();
        }

        private Library library;
        private SongCatalogue catalogue;

        internal JukeController()
        {
            library = new Library();
            catalogue = new SongCatalogue(library);
            //AsyncSongLoader.LoadCompleted += AsyncSongLoader_LoadCompleted;
            Player = new Player(library);
        }

        public void Dispose()
        {
            Player.Dispose();
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

        public void SaveLibrary(SongWriter writer)
        {
            writer.Write(library.Songs);
        }

        public void LoadLibrarySync(SongLoader loader)
        {
            LoadHandler.LoadSongsSync(loader);
        }
        }
}
