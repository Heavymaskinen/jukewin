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
using ViewModelCommands;

namespace Juke.UI.Admin
{
    public class AdminViewModel : AdministratorModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ViewControl view;
        private JukeController controller;
        private string selectedArtist;
        private string selectedAlbum;
        private Song selectedSong;
        private IList<Song> selectedSongs;
        private readonly ProgressTracker progressTracker;

        public CancellationTokenSource CancelTokenSource { get; set; }
        public ObservableCollection<Song> Queue { get; private set; }
        public ObservableCollection<Song> Songs { get; private set; }
        public ObservableCollection<string> Albums { get; private set; }
        public ObservableCollection<string> Artists { get; private set; }
        public ICommand LoadSongs => new LoadSongsCommand(controller, view, this);
        public ICommand LoadLibrary => new LoadLibraryCommand(controller, view, this);
        public ICommand SaveLibrary => new SaveLibraryCommand(controller, view, this);
        public ICommand PlaySong => new PlaySongCommand(controller, view, this);
        public ICommand StopSong => new StopSongCommand(controller, view, this);
        public ICommand SkipSong => new StopSongCommand(controller, view, this);
        public ICommand EditSong => new EditSongCommand(controller, view, this);
        public ICommand EditAlbum => new EditAlbumCommand(controller, view, this);
        public ICommand RenameArtist => new RenameArtistCommand(controller, view, this);
        public ICommand DeleteArtist => new DeleteArtistCommand(controller, view, this);
        public ICommand DeleteAlbum => new DeleteAlbumCommand(controller, view, this);
        public ICommand DeleteSong => new DeleteSongCommand(controller, view, this);

        public ICommand CancelLoad => new RelayCommand(
            (obj) => CancelTokenSource != null && !CancelTokenSource.IsCancellationRequested,
            (obj) =>
            {
                CancelTokenSource.Cancel();
                CancelTokenSource.Dispose();
                CancelTokenSource = null;
            });

        public ProgressTracker ProgressTracker => progressTracker;

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
            view = viewControl;
            controller = JukeController.Instance;
            progressTracker = new ProgressTracker(controller.LoadHandler);
            progressTracker.Changed += ProgressTracker_Changed;
            InitializeCollections();
            controller.LoadHandler.LibraryUpdated += LoadHandlerOnLibraryUpdated;
            controller.LoadHandler.LoadCompleted += (sender, args) => view.CommandCompleted(null);
            Messenger.FrontendMessagePosted += Messenger_FrontendMessagePosted;
            selectedAlbum = Song.ALL_ALBUMS;
            selectedArtist = Song.ALL_ARTISTS;
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
            if (artist == null) artist = Song.ALL_ARTISTS;
            var songs = controller.Browser.Songs;
            if (artist != Song.ALL_ARTISTS)
            {
                RefreshAlbums(controller.Browser.GetAlbumsByArtist(artist));
                songs = selectedAlbum != Song.ALL_ALBUMS
                    ? controller.Browser.GetSongsByArtistAndAlbum(artist, selectedAlbum)
                    : controller.Browser.GetSongsByArtist(artist);
            }
            else
            {
                if (SelectedAlbum != Song.ALL_ALBUMS)
                {
                    songs = controller.Browser.GetSongsByAlbum(SelectedAlbum);
                }

                RefreshAlbums(controller.Browser.Albums);
                selectedAlbum = SelectedAlbum;
            }

            RefreshSongs(songs);
        }

        private void SelectAlbum(string album)
        {
            if (album == null) album = Song.ALL_ALBUMS;
            var songs = controller.Browser.Songs;
            if (album != Song.ALL_ALBUMS)
            {
                songs = controller.Browser.GetSongsByAlbum(album);
            }
            else if (album == Song.ALL_ALBUMS && SelectedArtist != Song.ALL_ARTISTS)
            {
                songs = controller.Browser.GetSongsByArtist(SelectedArtist);
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
                Albums.Insert(0, Song.ALL_ALBUMS);
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