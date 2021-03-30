using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Core
{
    public interface LibraryBrowser
    {
        IList<Song> Songs { get; }
        IList<string> Artists { get; }
        IList<string> Albums { get; }

        IList<Song> GetSongsByArtist(string artistName);

        IList<Song> GetSongsByAlbum(string albumName);

        IList<string> GetAlbumsByArtist(string artistName);

        IList<Song> GetSongsByTitle(string title);

        IList<Song> GetSongsByArtistAndTitle(string artistName, string songTitle);
        IList<Song> GetSongsByArtistAndAlbum(string artist, string selectedAlbum);
    }
}