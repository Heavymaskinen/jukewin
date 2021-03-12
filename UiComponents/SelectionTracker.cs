using System;
using System.Collections.Generic;
using System.Linq;
using DataModel;
using Juke.Core;

namespace Juke.UI
{

    public class SelectionTracker
    {
        public event EventHandler<string> Changed;

        private Dictionary<string, Dictionary<string, IList<Song>>> collection;

        private string selectedArtist;
        private string selectedAlbum;
        private Song selectedSong;
        private IList<Song> selectedSongs;
        private LibraryBrowser browser;
        
        public IList<Song> Songs { get; set; }
        public IList<string> Albums { get; set; }
        public IList<string> Artists { get; set; }

        public string SelectedArtist
        {
            get => selectedArtist;
            set
            {
                selectedArtist = value ?? Song.ALL_ARTISTS;
                SelectArtist(selectedArtist);
                RaisePropertyChanged(nameof(SelectedArtist));
                RaisePropertyChanged(nameof(SelectedAlbum));
                RaisePropertyChanged(nameof(SelectedSong));
            }
        }

        public string SelectedAlbum
        {
            get => selectedAlbum;
            set
            {
                selectedAlbum = value ?? Song.ALL_ALBUMS;
                SelectAlbum(selectedAlbum);
                selectedSong = Songs.Count > 0 ? Songs[0] : null;
                RaisePropertyChanged(nameof(SelectedAlbum));
            }
        }

        public Song SelectedSong
        {
            get => selectedSong;
            set
            {
                selectedSong = value;
                selectedAlbum = selectedSong.Album;
                selectedArtist = selectedSong.Artist;
                RaisePropertyChanged(nameof(SelectedSong));
                RaisePropertyChanged(nameof(SelectedAlbum));
                RaisePropertyChanged(nameof(SelectedArtist));
            }
        }

        public IList<Song> SelectedSongs
        {
            get => selectedSongs;
            set => selectedSongs = value;
        }

        public SelectionTracker(LibraryBrowser browser)
        {
            this.browser = browser;
            selectedAlbum = Song.ALL_ALBUMS;
            SelectedArtist = Song.ALL_ARTISTS;
            selectedSongs = new List<Song>();
            Refresh(browser);
        }

        public void Refresh(LibraryBrowser browser)
        {
            Songs = browser.Songs;
            Albums = browser.Albums;
            Artists = browser.Artists;
            collection = new Dictionary<string, Dictionary<string, IList<Song>>>();
            foreach (var song in Songs)
            {
                if (!collection.ContainsKey(song.Artist))
                {
                    collection.Add(song.Artist, new ());
                }
                if (!collection[song.Artist].ContainsKey(song.Album))
                {
                    collection[song.Artist].Add(song.Album, new List<Song>());
                }
                
                collection[song.Artist][song.Album].Add(song);
            }
        }

        private void SelectArtist(string artist)
        {
            var songs = browser.Songs;
            if (artist != Song.ALL_ARTISTS)
            {
                RefreshAlbums(GetAlbumsByArtist(artist));
                if (!Albums.Contains(selectedAlbum))
                {
                    selectedAlbum = Albums[0];
                }

                if (SelectedAlbum != Song.ALL_ALBUMS)
                {
                    songs = selectedAlbum != Song.ALL_ALBUMS
                    ? collection[artist][selectedAlbum]
                    : GetAllSongsByArtist(artist);
                    selectedSong = songs[0];
                } else
                {
                    songs = GetAllSongsByArtist(artist);
                }

            }
            else
            {
                if (SelectedAlbum != Song.ALL_ALBUMS)
                {
                    songs = GetSongsByAlbum(SelectedAlbum);
                }

                RefreshAlbums(browser.Albums);
                selectedAlbum = SelectedAlbum;
            }

            RefreshSongs(songs);
        }

        private IList<Song> GetSongsByAlbum(string album)
        {
            var songs = new List<Song>();
            foreach (var artist in collection)
            {
                if (artist.Value.ContainsKey(album))
                {
                    songs.AddRange(artist.Value[album]);
                }
            }

            return songs;
        }

        private IList<string> GetAlbumsByArtist(string artist)
        {
            return collection[artist].Keys.ToList();
        }

        private List<Song> GetAllSongsByArtist(string artist)
        {
            var selSongs = new List<Song>();
            foreach (var album in collection[artist].Keys)
            {
                selSongs.AddRange(collection[artist][album]);
            }

            return selSongs;
        }

        private void SelectAlbum(string album)
        {
            var songs = browser.Songs;
            if (album != Song.ALL_ALBUMS)
            {
                if (SelectedArtist != Song.ALL_ARTISTS)
                {
                    songs = collection[SelectedArtist][album];
                } else
                {
                    songs = GetSongsByAlbum(album);
                }
                
            }
            else if (album == Song.ALL_ALBUMS && SelectedArtist != Song.ALL_ARTISTS)
            {
                songs = GetAllSongsByArtist(SelectedArtist);
            }

            RefreshSongs(songs);
        }

        private void RefreshSongs(IList<Song> songs)
        {
            Songs = songs;
            RaisePropertyChanged(nameof(Songs));
        }

        private void RefreshArtists()
        {
            Artists = new List<string>(browser.Artists.OrderBy(s => s));
            if (Artists.Count > 1)
            {
                Artists.Insert(0, Song.ALL_ARTISTS);
            }

            RaisePropertyChanged(nameof(Artists));
        }

        private void RefreshAlbums(IList<string> albums)
        {
            Albums = new List<string>(albums.OrderBy(s => s));
            if (Albums.Count > 1)
            {
                Albums.Insert(0, Song.ALL_ALBUMS);
            }

            RaisePropertyChanged(nameof(Albums));
        }
        
        private void RaisePropertyChanged(string propertyName)
        {
            Changed?.Invoke(this, propertyName);
        }
    }
}