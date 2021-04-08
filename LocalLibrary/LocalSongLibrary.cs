using System;
using System.Collections.Generic;
using System.Linq;
using Juke.Core;
using JukeApiLibrary;
using JukeApiModel;

namespace LocalRepositori
{
    public class LocalSongLibrary : SongLibrary
    {
        private LibraryBrowser libraryBrowser;

        public LocalSongLibrary(LibraryBrowser libraryBrowser)
        {
            this.libraryBrowser = libraryBrowser;
        }

        public List<ApiSong> GetAllSongs()
        {
            return libraryBrowser.Songs.Select(s => new ApiSong(s.Artist, s.Album, s.Name)).ToList();
        }
    }
}