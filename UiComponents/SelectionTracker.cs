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
        
        private string selectedArtist;
        private string selectedAlbum;
        private Song selectedSong;
        private IList<Song> selectedSongs;
        private LibraryBrowser browser;
        
        public IList<Song> Songs { get; private set; }
        public IList<string> Albums { get; private set; }
        public IList<string> Artists { get; set; }

        public string SelectedArtist
        {
            get => selectedArtist;
            set
            {
                selectedArtist = value;
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
                selectedAlbum = value;
                SelectAlbum(selectedAlbum);
                selectedSong = Songs[0];
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
            selectedSongs = new List<Song>();
            Songs = browser.Songs;
            Albums = browser.Albums;
            Artists = browser.Artists;
        }

        private void SelectArtist(string artist)
        {
            if (artist == null) artist = Song.ALL_ARTISTS;
            
            var songs = browser.Songs;
            if (artist != Song.ALL_ARTISTS)
            {
                RefreshAlbums(browser.GetAlbumsByArtist(artist));
                if (!Albums.Contains(selectedAlbum))
                {
                    selectedAlbum = Albums[0];
                }

                songs = selectedAlbum != Song.ALL_ALBUMS
                    ? browser.GetSongsByArtistAndAlbum(artist, selectedAlbum)
                    : browser.GetSongsByArtist(artist);
                selectedSong = songs[0];
            }
            else
            {
                if (SelectedAlbum != Song.ALL_ALBUMS)
                {
                    songs = browser.GetSongsByAlbum(SelectedAlbum);
                }

                RefreshAlbums(browser.Albums);
                selectedAlbum = SelectedAlbum;
            }

            RefreshSongs(songs);
        }

        private void SelectAlbum(string album)
        {
            if (album == null) album = Song.ALL_ALBUMS;
            var songs = browser.Songs;
            if (album != Song.ALL_ALBUMS)
            {
                songs = browser.GetSongsByAlbum(album);
            }
            else if (album == Song.ALL_ALBUMS && SelectedArtist != Song.ALL_ARTISTS)
            {
                songs = browser.GetSongsByArtist(SelectedArtist);
            }

            RefreshSongs(songs);
        }

        private void RefreshSongs(IList<Song> songs)
        {
            Songs = new List<Song>(songs);
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