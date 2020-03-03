using DataModel;
using Juke.Control;
using Juke.Core;

using Juke.IO;
using Juke.UI.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Juke.UI
{
    public class JukeViewModel : INotifyPropertyChanged
    {
        private JukeController controller;
        private ViewControl view;
        private string selectedArtist;
        private string selectedAlbum;
        private Song selectedSong;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Song> Queue { get; private set; }
        public ObservableCollection<Song> Songs { get; private set; }
        public ObservableCollection<string> Albums { get; private set; }
        public ObservableCollection<string> Artists { get; private set; }

        
        public string SelectedArtist { get { return selectedArtist; }
            set
            {
                selectedArtist = value;
                SelectArtist(selectedArtist);
            }
        }
        
        public string SelectedAlbum
        {
            get { return selectedAlbum; }
            set
            {
                selectedAlbum = value;
                SelectAlbum(selectedAlbum);
            }
        }

        public Song SelectedSong
        {
            get { return selectedSong; }
            set
            {
                selectedSong = value;
                RaisePropertyChanged("SelectedSong");
            }
        }

        public string CurrentSong
        {
            get
            {
                var song = controller.Player.NowPlaying;
                if (song == null)
                {
                    return "<None>";
                }
                return song.Name + " (" + song.Artist + ")";
            }
        }

        public string SystemMessage { set; get; }

        public ICommand LoadSongs { get { return new LoadSongsCommand(controller, view, this); } }
        public ICommand LoadLibrary { get { return new LoadLibraryCommand(controller, view, this); } }
        public ICommand SaveLibrary { get { return new SaveLibraryCommand(controller, view, this); } }
        public ICommand PlaySong { get { return new PlaySongCommand(controller, view, this); } }
        public ICommand SkipSong { get { return new SkipSongCommand(controller, view, this); } }

        public JukeViewModel(ViewControl view, PlayerEngine player)
        {
            controller = JukeController.Instance;
            controller.Player.RegisterPlayerEngine(player);
            this.view = view;
            InitializeCollections();

            AsyncSongLoader.LoadProgress += AsyncSongLoader_LoadProgress;
            controller.LoadHandler.LibraryUpdated += LoadHandler_LibraryUpdated;
            Player.SongPlayed += Player_SongPlayed;
        }

        private void Player_SongPlayed(object sender, Song played)
        {
            RefreshQueue();
            RaisePropertyChanged("CurrentSong");
        }

        private void LoadHandler_LibraryUpdated(object sender, EventArgs e)
        {
            InitializeCollections();
        }

        private void InitializeCollections()
        {

            Artists = new ObservableCollection<string>(controller.Browser.Artists);
            RaisePropertyChanged("Artists");

            RefreshAlbums(controller.Browser.Albums);
            RefreshSongs(controller.Browser.Songs);
            RefreshQueue();
        }

        private void RefreshQueue()
        {
            Queue = new ObservableCollection<Song>(controller.Player.Queue.Songs);
            RaisePropertyChanged("Queue");
        }

        private void RefreshAlbums(IList<string> albums)
        {
            Albums = new ObservableCollection<string>(albums);
            RaisePropertyChanged("Albums");
        }

        private void RefreshSongs(IList<Song> songs)
        {
            Songs = new ObservableCollection<Song>(songs);
            RaisePropertyChanged("Songs");
        }

        private void AsyncSongLoader_LoadProgress(object sender, string e)
        {
            SystemMessage = e;
            RaisePropertyChanged("SystemMessage");
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SelectArtist(string artist)
        {
            IList<string> albums;
            if (string.IsNullOrEmpty(selectedArtist) || artist == Song.ALL_ARTISTS)
            {
                albums = controller.Browser.Albums;
            }
            else
            {
                albums = controller.Browser.GetAlbumsByArtist(selectedArtist);
            }

            RefreshAlbums(albums);
        }

        private void SelectAlbum(string album)
        {
            IList<Song> songs;
            if (string.IsNullOrEmpty(album) || album == Song.ALL_ALBUMS)
            {
                songs = controller.Browser.Songs;
            }
            else
            {
                songs = controller.Browser.GetSongsByAlbum(album);
            }

            RefreshSongs(songs);
        }

    }

}
