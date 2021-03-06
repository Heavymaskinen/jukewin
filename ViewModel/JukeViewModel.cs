﻿using DataModel;
using Juke.Control;
using Juke.Core;
using Juke.IO;
using Juke.UI.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace Juke.UI
{
    public class JukeViewModel : SelectionModel
    {
        private IJukeController controller;

        public CancellationTokenSource CancelTokenSource { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Song> Queue { get; private set; }
        public ObservableCollection<Song> Songs { get; private set; }
        public ObservableCollection<string> Albums { get; private set; }

        public ObservableCollection<string> Artists { get; private set; }

        public IList<Song> GetSongsByArtist(string art)
        {
            return controller.Browser.GetSongsByArtist(art);
        }

        public ProgressTracker ProgressTracker => progressTracker;

        public double ProgressMax => progressTracker.ProgressMax;

        public double Progress
        {
            get { return progressTracker.Progress; }
            set { progressTracker.Progress = value; }
        }

        public bool ProgressIndeterminate => progressTracker.IsIndeterminate;

       
        public string SystemMessage { set; get; }

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

        public ViewControl View { get; set; }
        public IList<Song> SelectedSongs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SelectionTracker SelectionTracker { get; }

        private ProgressTracker progressTracker;

        public JukeViewModel(ViewControl view)
        {
            controller = JukeController.Instance;
            progressTracker = new ProgressTracker(controller.LoadHandler);
            progressTracker.Changed += ProgressTracker_Changed;
            View = view;
            InitializeCollections();

            SelectionTracker = new SelectionTracker(controller.Browser);
            SelectionTracker.Changed += SelectionTracker_Changed;
            controller.LoadHandler.LibraryUpdated += LoadHandler_LibraryUpdated;
            controller.LoadHandler.LoadCompleted += (sender, args) => View.CommandCompleted(null);
            controller.Player.SongPlayed += Player_SongPlayed;
            Messenger.FrontendMessagePosted += Messenger_FrontendMessagePosted;
        }

        private void SelectionTracker_Changed(object sender, string e)
        {
            RaisePropertyChanged(nameof(SelectionTracker));
        }

        private void ProgressTracker_Changed(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(ProgressTracker));
        }

        public void Dispose()
        {
            LoaderCancellationTokenProvider.Dispose();
            controller.Dispose();
        }

        private void Messenger_FrontendMessagePosted(string message, Messenger.TargetType target)
        {
            SystemMessage = message;
            RaisePropertyChanged(nameof(SystemMessage));
        }

        private void Player_SongPlayed(object sender, Song played)
        {
            RefreshQueue();
            RaisePropertyChanged("CurrentSong");
        }

        private void LoadHandler_LibraryUpdated(object sender, EventArgs e)
        {
            SelectionTracker.Refresh(controller.Browser);
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
            Queue = new ObservableCollection<Song>(controller.Player.EnqueuedSongs);
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
            //Progress += progress;
            RaisePropertyChanged(nameof(Progress));
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}