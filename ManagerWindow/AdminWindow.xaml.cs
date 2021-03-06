﻿using System.ComponentModel;
using DataModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Juke.Control;
using Juke.External.Wmp;
using Juke.UI.Command;
using Juke.UI.Admin;
using ContextMenu = System.Windows.Controls.ContextMenu;
using System.Collections.Generic;
using System.Linq;

namespace Juke.UI.Wpf
{
    public enum EditType
    {
        Artist,
        Album,
        Song
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window, ViewControl
    {
        private AdminViewModel viewModel;

        public AdminWindow()
        {
            Construct(new AdminViewModel(this));
        }

        private void Construct(AdminViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
            (DataContext as AdminViewModel).PropertyChanged += MainWindow_PropertyChanged;
            songBox.SelectionChanged += SongBox_SelectionChanged;
        }

        private void SongBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var songs = songBox.SelectedItems.Cast<Song>().ToList();
            viewModel.SelectionTracker.SelectedSongs = songs;
        }

        public AdminWindow(AdminViewModel viewModel)
        {
            InitializeComponent();
            Construct(viewModel);
        }

        private void MainWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Albums")
            {
                Dispatcher.Invoke(UpdateAlbums);
            }
            else if (e.PropertyName == "Songs")
            {
                Dispatcher.Invoke(UpdateSongs);
            }
            else if (e.PropertyName == "Artists")
            {
                Dispatcher.Invoke(UpdateArtists);
            }
        }

        private void UpdateArtists()
        {
            //artistBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            artistBox.Items.Refresh();
        }

        private void UpdateSongs()
        {
            //songBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            songBox.Items.Refresh();
        }

        private void UpdateAlbums()
        {
            //albumBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            albumBox.Items.Refresh();
        }

        public string PromptPath()
        {
            Messenger.Log("Prompt for path");
            
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Messenger.Log("Path selected: " + dlg.SelectedPath);
                return dlg.SelectedPath;
            }

            return null;
        }

        public SongUpdate PromptSongData(InfoType infoType)
        {
            Song song;

            if (infoType == InfoType.Album && viewModel.SelectedAlbum != Song.ALL_ALBUMS)
            {
                var artist = viewModel.SelectedArtist != Song.ALL_ARTISTS ? viewModel.SelectedArtist : null;
                if (artist == null && viewModel.Songs.Count>0)
                {
                    artist = viewModel.Songs[0].Artist;
                }
                Messenger.Log("Editing album");
                song = new Song(artist, viewModel.SelectedAlbum, null);
                var infoWindow = new InfoWindow(new SongUpdate(song), infoType);
                var useData = infoWindow.ShowDialog();
                return useData == true ? infoWindow.SongData : null;
            }
            
            if (infoType == InfoType.Artist && viewModel.SelectedArtist != Song.ALL_ARTISTS)
            {
                Messenger.Log("Editing artist");
                song = new Song(viewModel.SelectedArtist, null, null);
                var infoWindow = new InfoWindow(new SongUpdate(song), infoType);
                var useData = infoWindow.ShowDialog();
                return useData == true ? infoWindow.SongData : null;
            }

            if (songBox.SelectedItem != null)
            {
                song = songBox.SelectedItem as Song;
            }
            else if (artistBox.SelectedItem != null)
            {
                if (artistBox.SelectedItem?.ToString() == Song.ALL_ARTISTS ||
                    albumBox.SelectedItem?.ToString() == Song.ALL_ALBUMS) return null;
                
                song = CreateSongDataFromAlbumAndArtist();
            }
            else
            {
                song = null;
            }

            if (song == null)
            {
                Messenger.Log("Song not found?");
                return null;
            }

            if (infoType == InfoType.Song)
            {
                if (song.Album == "<unknown>" || song.Artist == "<unknown>")
                {
                    song = PopulateSongDataFromTags(song);
                }
            }

            var dialog = new InfoWindow(new SongUpdate(song), infoType);

            var result = dialog.ShowDialog();
            return result == true ? dialog.SongData : null;
        }

        private static Song PopulateSongDataFromTags(Song song)
        {
            var tagFactory = new WmpTagReaderFactory();
            var tag = tagFactory.Create(song.FilePath);
            song = new Song(tag.Artist, tag.Album, tag.Title, song.TrackNo, song.FilePath);
            return song;
        }

        private Song CreateSongDataFromAlbumAndArtist()
        {
            Song song = null;

            if (albumBox.SelectedItem != null)
            {
                song = new Song(artistBox.SelectedItem.ToString(), albumBox.SelectedItem.ToString(), "");
            }
            else
            {
                song = new Song(artistBox.SelectedItem.ToString(), "", "");
            }

            return song;
        }

        public void CommandCompleted(JukeCommand command)
        {
            UpdateArtists();
            UpdateAlbums();
            UpdateSongs();
        }

        private void ArtistBox_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (viewModel.SelectedArtist == null) return;

            ContextMenu menu = FindResource("artistContextMenu") as ContextMenu;
            menu.PlacementTarget = artistBox;
            menu.IsOpen = true;
        }

        private void AlbumBox_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (viewModel.SelectedAlbum == null) return;

            ContextMenu menu = FindResource("albumContextMenu") as ContextMenu;
            menu.PlacementTarget = albumBox;
            menu.IsOpen = true;
        }

        private void SongBox_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (viewModel.SelectionTracker.SelectedSong == null) return;

            ContextMenu menu = FindResource("songContextMenu") as ContextMenu;
            menu.PlacementTarget = songBox;
            menu.IsOpen = true;
        }
    }
}