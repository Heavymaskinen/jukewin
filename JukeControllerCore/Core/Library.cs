using DataModel;
using Juke.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Core
{
    public class Library : LibraryBrowser
    {
        public IList<Song> Songs { get; }
        public IList<string> Artists { get; }
        public IList<string> Albums { get; }

        public Library()
        {
            Songs   = new List<Song>();
            Artists = new List<string>();
            Albums  = new List<string>();
        }

        public void AddSong(Song song)
        {
            if (song == null)
            {
                return;
            }

            if (song.ID == 0)
            {
                song.ID = Songs.Count + 1;
            }
            
            if (!Songs.Contains(song))
            {
                Songs.Add(song);
            }

            if (!Albums.Contains(song.Album))
            {
                Albums.Add(song.Album);
            }

            if (!Artists.Contains(song.Artist))
            {
                Artists.Add(song.Artist);
            }
        }

        public void UpdateSong(SongUpdate edit)
        {
            DeleteSong(edit.SongSource);
            AddSong(CreateSongFromUpdate(edit));
        }

        internal void Clear()
        {
            Songs.Clear();
        }

        public void DeleteSong(Song song)
        {
            Songs.Remove(song);

            if (GetSongsByArtist(song.Artist).Count == 0)
            {
                Artists.Clear();
            }

            if (GetSongsByAlbum(song.Album).Count == 0)
            {
                Albums.Clear();
            }
        }

        public IList<Song> GetSongsByArtist(string artistName)
        {
            var foundSongs = new List<Song>();
            foreach (var song in Songs)
            {
                if (song.Artist == artistName)
                {
                    foundSongs.Add(song);
                }
            }

            return foundSongs;
        }

        public IList<Song> GetSongsByAlbum(string albumName)
        {
            var songList = new List<Song>();
            foreach (var song in Songs)
            {
                if (song.Album == albumName)
                {
                    songList.Add(song);
                }
            }

            songList.Sort();

            return songList;
        }

        public IList<string> GetAlbumsByArtist(string artistName)
        {
            var albumList = new List<string>();
            foreach (var song in Songs)
            {
                if (song.Artist.Equals(artistName) && !albumList.Contains(song.Album))
                {
                    albumList.Add(song.Album);
                }
            }

            return albumList;
        }

        public IList<Song> GetSongsByTitle(string title)
        {
            return Songs.Where(song => song.Name == title).ToList();
        }

        public IList<Song> GetSongsByArtistAndTitle(string artistName, string songTitle)
        {
            return Songs.Where(song => song.Artist == artistName && song.Name == songTitle).ToList();
        }

        public Song GetSongByID(int id)
        {
            return Songs.First(song => song.ID == id);
        }

        private Song CreateSongFromUpdate(SongUpdate edit)
        {
            return new Song(edit.NewArtist, edit.NewAlbum, edit.NewName, edit.NewTrackNo, edit.SongSource.FilePath)
                {ID = edit.SongSource.ID};
        }
    }
}