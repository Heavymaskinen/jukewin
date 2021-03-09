using DataModel;
using Juke.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Core
{
    class Library : LibraryBrowser
    {
        public IList<Song> Songs
        {
            get
            {
                var list = songCore.Values.ToList();
                list.Sort(Song.Comparison);
                return list;
            }
        }

        public IList<string> Artists { get; private set; }
        public IList<string> Albums { get; private set; }

        private Dictionary<string, Song> songCore;

        public Library()
        {
            Artists = new List<string>();
            Albums = new List<string>();
            songCore = new Dictionary<string, Song>();
        }

        public void AddSong(Song song)
        {
            if (!songCore.ContainsKey(song.ID))
            {
                songCore.Add(song.ID, song);
            }
            else if (!songCore[song.ID].Equals(song))
            {
                songCore[song.ID].Merge(song);
            }
        }
        
        public void InitialiseParts()
        {
            Albums.Clear();
            Artists.Clear();
            foreach (var song in Songs)
            {
                if (!Albums.Contains(song.Album))
                {
                    Albums.Add(song.Album);
                }

                if (!Artists.Contains(song.Artist))
                {
                    Artists.Add(song.Artist);
                }
            }
        }

        public void UpdateSong(SongUpdate edit)
        {
            Songs.Remove(edit.SongSource);
            Songs.Add(CreateSongFromUpdate(edit));
        }

        internal void RemoveById(string id)
        {
            songCore.Remove(id);
        }

        internal void Clear()
        {
            Songs.Clear();
        }

        public void DeleteSong(Song song)
        {
            RemoveById(song.ID);
            InitialiseParts();
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
            var songList = Songs.Where(song => song.Album == albumName).ToList();

            songList.Sort(Song.Comparison);
            return songList;
        }

        public IList<string> GetAlbumsByArtist(string artistName)
        {
            var albumList = new List<string>();
            if (Songs.Count == 0)
            {
                return albumList;
            }

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
            var songList = new List<Song>();
            foreach (var song in Songs)
            {
                if (song.Artist == artistName && song.Name == songTitle)
                {
                    songList.Add(song);
                }
            }

            return songList;
        }

        private Song CreateSongFromUpdate(SongUpdate edit)
        {
            return new Song(edit.NewArtist, edit.NewAlbum, edit.NewName, edit.NewTrackNo, edit.SongSource.FilePath);
        }
    }
}
