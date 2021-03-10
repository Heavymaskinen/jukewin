using System;
using DataModel;
using Juke.Control;
using Juke.UI.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using UiComponents;

namespace Juke.UI.Admin
{
    public class AdminViewModel : SelectionModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ViewControl View;
        private JukeController controller;
        private string selectedArtist;
        private string selectedAlbum;
        private Song selectedSong;
        private ProgressTracker progressTracker;

        public CancellationTokenSource CancelTokenSource { get; set; }
        public ObservableCollection<Song> Queue { get; private set; }
        public ObservableCollection<Song> Songs { get; private set; }
        public ObservableCollection<string> Albums { get; private set; }
        public ObservableCollection<string> Artists { get; private set; }
        public ICommand LoadSongs => new LoadSongsCommand(controller, View, this);
        public ICommand LoadLibrary => new LoadLibraryCommand(controller, View, this);
        public ICommand SaveLibrary => new SaveLibraryCommand(controller, View, this);
        public ICommand PlaySong => new PlaySongCommand(controller, View, this);
        public ICommand StopSong => new StopSongCommand(controller, View, this);
        public ICommand SkipSong => new StopSongCommand(controller, View, this);
        public ICommand EditSong => new EditSongCommand(controller, View, this);
        public ICommand EditAlbum => new EditAlbumCommand(controller, View, this);
        public ICommand RenameArtist => new RenameArtistCommand(controller, View, this);
        public ICommand DeleteAlbum => new DeleteAlbumCommand(controller, View, this);
        public ICommand DeleteSong => new DeleteSongCommand(controller, View, this);
        public ICommand CancelLoad => new RelayCommand(
            (obj) => CancelTokenSource != null && !CancelTokenSource.IsCancellationRequested,
            (obj) =>
            {
                CancelTokenSource.Cancel();
                CancelTokenSource.Dispose();
                CancelTokenSource = null;
            });
        public ProgressTracker ProgressTracker => progressTracker;

        public double ProgressMax => ProgressTracker.ProgressMax;

        public double Progress
        {
            get
            {
                return progressTracker.Progress;
            }
            set
            {
                progressTracker.Progress = value;
            }
        }

        public bool ProgressIndeterminate => progressTracker.IsIndeterminate;
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

        public AdminViewModel(ViewControl viewControl)
        {
            View = viewControl;
            controller = JukeController.Instance;
            progressTracker = new ProgressTracker(controller.LoadHandler);
            progressTracker.Changed += ProgressTracker_Changed;
            InitializeCollections();
            controller.LoadHandler.LibraryUpdated += LoadHandlerOnLibraryUpdated;
            Messenger.FrontendMessagePosted += Messenger_FrontendMessagePosted;
        }

        private void LoadHandlerOnLibraryUpdated(object sender, EventArgs e)
        {
            InitializeCollections();
        }

        private void ProgressTracker_Changed(object sender, System.EventArgs e)
        {
            RaisePropertyChanged(nameof(ProgressTracker));
        }

        private void Messenger_FrontendMessagePosted(string message, Messenger.TargetType target)
        {
            SystemMessage = message;
            RaisePropertyChanged(nameof(SystemMessage));
        }

        private void InitializeCollections()
        {
            RefreshArtists();
            RefreshAlbums(controller.Browser.Albums);
            RefreshSongs(controller.Browser.Songs);
            RefreshQueue();
        }

        private void RefreshArtists()
        {
            Artists = new ObservableCollection<string>(controller.Browser.Artists.OrderBy(s => s));
            RaisePropertyChanged(nameof(Artists));
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

        private void RefreshSongs(IList<Song> songs)
        {
            Songs = new ObservableCollection<Song>(songs);
            RaisePropertyChanged(nameof(Songs));
        }

        private void RefreshAlbums(IList<string> albums)
        {
            Albums = new ObservableCollection<string>(albums.OrderBy(s => s));
            if (Albums.Count > 1)
            {
                Albums.Insert(0,Song.ALL_ALBUMS);
            }
            RaisePropertyChanged(nameof(Albums));
        }

        private void RefreshQueue()
        {
            Queue = new ObservableCollection<Song>(controller.Player.Queue.Songs);
            RaisePropertyChanged(nameof(Queue));
        }
    }
}
