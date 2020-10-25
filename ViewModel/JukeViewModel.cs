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
using System.Windows.Input;

namespace Juke.UI
{
    public class JukeViewModel : INotifyPropertyChanged
    {
        public enum InfoType
        {
            Song, Album, Artist
        }

        private JukeController controller;
        private string selectedArtist;
        private string selectedAlbum;
        private Song selectedSong;
        private ViewControl view;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Song> Queue { get; private set; }
        public ObservableCollection<Song> Songs { get; private set; }
        public ObservableCollection<string> Albums { get; private set; }

        public IList<Song> GetSongsByArtist(string art)
        {
            return controller.Browser.GetSongsByArtist(art);
        }

        public ObservableCollection<string> Artists { get; private set; }

        public double ProgressMax { get; set; }
        public double Progress { get; set; }

        public bool ProgressIndeterminate { get; set; }


        public string SelectedArtist
        {
            get { return selectedArtist; }
            set
            {
                selectedArtist = value;
                SelectArtist(selectedArtist);
                RaisePropertyChanged("SelectedArtist");
            }
        }

        public string SelectedAlbum
        {
            get { return selectedAlbum; }
            set
            {
                selectedAlbum = value;
                SelectAlbum(selectedAlbum);
                RaisePropertyChanged("SelectedAlbum");
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
        public ICommand EditSong { get { return new EditSongCommand(controller, view, this); } }
        public ICommand EditAlbum { get { return new EditAlbumCommand(controller, view, this); } }
        public ICommand RenameArtist { get { return new RenameArtistCommand(controller, view, this); } }
        public ICommand DeleteAlbum { get { return new DeleteAlbumCommand(controller, view, this); } }
        public ICommand DeleteSong { get { return new DeleteSongCommand(controller, view, this); } }

        public ViewControl View { get => view; set => view = value; }

        public JukeViewModel(ViewControl view, PlayerEngine playerEngine)
        {
            controller = JukeController.Instance;

            controller.Player.RegisterPlayerEngine(playerEngine);
            this.view = view;
            InitializeCollections();
            Progress = 0;
            ProgressMax = 100;
            ProgressIndeterminate = false;
            AsyncSongLoader.LoadProgress += AsyncSongLoader_LoadProgress;
            AsyncSongLoader.LoadInitiated += AsyncSongLoader_LoadInitiated;
            AsyncSongLoader.NewLoad += AsyncSongLoader_NewLoad;
            controller.LoadHandler.LibraryUpdated += LoadHandler_LibraryUpdated;
            controller.LoadHandler.LoadInitiated += AsyncSongLoader_LoadInitiated;
            controller.LoadHandler.NewLoad += AsyncSongLoader_NewLoad;
            controller.LoadHandler.LoadProgress += AsyncSongLoader_LoadProgress;
            Player.SongPlayed += Player_SongPlayed;
            Messenger.FrontendMessagePosted += Messenger_FrontendMessagePosted;
        }

        public void Dispose()
        {
            controller.Dispose();
        }

        private void Messenger_FrontendMessagePosted(string message, Messenger.TargetType target)
        {
            SystemMessage = message;
            RaisePropertyChanged("SystemMessage");
        }

        private void AsyncSongLoader_NewLoad(object sender, EventArgs e)
        {
            Progress = 0;
            ProgressMax = 0;
            ProgressIndeterminate = true;
            RaisePropertyChanged("Progress");
            RaisePropertyChanged("ProgressIndeterminate");
        }

        private void AsyncSongLoader_LoadInitiated(object sender, int load)
        {
            if (load > 0)
            {
                ProgressMax = load;
                Progress = 0;
                ProgressIndeterminate = false;
                Console.WriteLine("New max: " + ProgressMax);
                RaisePropertyChanged("ProgressMax");
                RaisePropertyChanged("Progress");
                RaisePropertyChanged("ProgressIndeterminate");
            }
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
            Artists = new ObservableCollection<string>(controller.Browser.Artists.OrderBy(s => s));
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
            Albums = new ObservableCollection<string>(albums.OrderBy(s => s));
            RaisePropertyChanged("Albums");
        }

        private void RefreshSongs(IList<Song> songs)
        {
            Songs = new ObservableCollection<Song>(songs);
            RaisePropertyChanged("Songs");
        }

        private void AsyncSongLoader_LoadProgress(object sender, int progress)
        {
            Progress += progress;
            RaisePropertyChanged("Progress");
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
                RefreshSongs(controller.Browser.Songs);
            }
            else
            {
                albums = controller.Browser.GetAlbumsByArtist(selectedArtist);
                RefreshSongs(controller.Browser.GetSongsByArtist(artist));
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
