﻿using DataModel;
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

        public void AddSong(Song song, bool reload)
        {
            if (!songCore.ContainsKey(song.ID))
            {
                songCore.Add(song.ID, song);
            }
        }

        public void AddSong(Song song) => AddSong(song, false);

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

        internal void RemoveById(string ID)
        {
            songCore.Remove(ID);
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
            var songList = new List<Song>();
            foreach (var song in Songs)
            {
                if (song.Album == albumName)
                {
                    songList.Add(song);
                }
            }

            songList.Sort(Song.Comparison);
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
            var songList = new List<Song>();
            foreach (var song in Songs)
            {
                if (song.Name == title)
                {
                    songList.Add(song);
                }
            }

            return songList;
        }

        public IList<Song> GetSongsByArtistAndTitle(string artistName, string songTitle)
        {
            var songList = new List<Song>();
            foreach (var song in Songs)
            {
                if (song.Artist == artistName && song.Name == song.Name)
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