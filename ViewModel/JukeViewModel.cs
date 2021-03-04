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
            get => selectedArtist;
            set
            {
                selectedArtist = value;
                SelectArtist(selectedArtist);
                RaisePropertyChanged(nameof(SelectedArtist));
            }
        }

        public string SelectedAlbum
        {
            get => selectedAlbum;
            set
            {
                selectedAlbum = value;
                SelectAlbum(selectedAlbum);
                RaisePropertyChanged(nameof(SelectedAlbum));
            }
        }

        public Song SelectedSong
        {
            get => selectedSong;
            set
            {
                selectedSong = value;
                RaisePropertyChanged(nameof(SelectedSong));
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

        public ICommand LoadSongs => new LoadSongsCommand(controller, View, this);
        public ICommand LoadLibrary => new LoadLibraryCommand(controller, View, this);
        public ICommand SaveLibrary => new SaveLibraryCommand(controller, View, this);
        public ICommand PlaySong => new PlaySongCommand(controller, View, this);
        public ICommand SkipSong => new SkipSongCommand(controller, View, this);
        public ICommand EditSong => new EditSongCommand(controller, View, this);
        public ICommand EditAlbum => new EditAlbumCommand(controller, View, this);
        public ICommand RenameArtist => new RenameArtistCommand(controller, View, this);
        public ICommand DeleteAlbum => new DeleteAlbumCommand(controller, View, this);
        public ICommand DeleteSong => new DeleteSongCommand(controller, View, this);

        public ViewControl View { get; set; }

        public JukeViewModel(ViewControl view, PlayerEngine playerEngine)
        {
            controller = JukeController.Instance;

            controller.Player.RegisterPlayerEngine(playerEngine);
            View = view;
            InitializeCollections();
            Progress = 0;
            ProgressMax = 100;
            ProgressIndeterminate = false;

            controller.LoadHandler.LibraryUpdated += LoadHandler_LibraryUpdated;
            controller.LoadHandler.LoadInitiated += AsyncSongLoader_LoadInitiated;
            controller.LoadHandler.NewLoad += AsyncSongLoader_NewLoad;
            controller.LoadHandler.LoadProgress += AsyncSongLoader_LoadProgress;
            controller.LoadHandler.LoadCompleted += (sender, args) => View.CommandCompleted(null);
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
            RaisePropertyChanged(nameof(SystemMessage));
        }

        private void AsyncSongLoader_NewLoad(object sender, EventArgs e)
        {
            Progress = 0;
            ProgressMax = 0;
            ProgressIndeterminate = true;
            RaisePropertyChanged(nameof(Progress));
            RaisePropertyChanged(nameof(ProgressIndeterminate));
        }

        private void AsyncSongLoader_LoadInitiated(object sender, int load)
        {
            if (load <= 0) return;
            ProgressMax = load;
            Progress = 0;
            ProgressIndeterminate = false;
            Console.WriteLine("New max: " + ProgressMax);
            RaisePropertyChanged("ProgressMax");
            RaisePropertyChanged(nameof(Progress));
            RaisePropertyChanged(nameof(ProgressIndeterminate));
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
            RaisePropertyChanged(nameof(Artists));
            RefreshAlbums(controller.Browser.Albums);
            RefreshSongs(controller.Browser.Songs);
            RefreshQueue();
        }
        private void RefreshQueue()
        {
            Queue = new ObservableCollection<Song>(controller.Player.Queue.Songs);
            RaisePropertyChanged(nameof(Queue));
        }

        private void RefreshAlbums(IList<string> albums)
        {
            Albums = new ObservableCollection<string>(albums.OrderBy(s => s));
            RaisePropertyChanged(nameof(Albums));
        }

        private void RefreshSongs(IList<Song> songs)
        {
            Songs = new ObservableCollection<Song>(songs);
            RaisePropertyChanged(nameof(Songs));
        }

        private void AsyncSongLoader_LoadProgress(object sender, int progress)
        {
            Progress += progress;
            RaisePropertyChanged(nameof(Progress));
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
