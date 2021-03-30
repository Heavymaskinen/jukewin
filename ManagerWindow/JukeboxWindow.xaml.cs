using DataModel;
using Juke.Control;
using Juke.Core;
using Juke.External.Wmp;
using Juke.UI.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Juke.UI.Admin;
using Juke.UI.SearchLogics;

namespace Juke.UI.Wpf
{
    /// <summary>
    /// Interaction logic for JukeboxWindow.xaml
    /// </summary>
    public partial class JukeboxWindow : ViewControl
    {
        private IJukeController jukeController;
        private AnimationTimeline fadeOutAnimation;
        private AnimationTimeline fadeInAnimation;
        private AnimationTimeline endlessFadeAnimation;
        private AnimationTimeline searchHideAnimation;
        private JukeViewModel viewModel;
        private bool loaded;

        private DispatcherTimer dispatchTimer;
        private SearchLogic searchLogic;
        private IList<SearchLogic> searchLogics;

        private WindowRouter windowRouter;

        public JukeboxWindow()
        {
            InitializeComponent();
            Messenger.Log("Starting UI");
            CreateFadeAnimation();

            viewModel = new JukeViewModel(this);
            DataContext = viewModel;
            loaded = false;
            windowRouter = new WindowRouter(this);

            dispatchTimer = new DispatcherTimer(
                TimeSpan.FromSeconds(3),
                DispatcherPriority.ApplicationIdle,
                (s, e) =>
                {
                    if (searchBox.Text.Length == 0)
                    {
                        searchBox.Visibility = Visibility.Hidden;
                    }
                },
                Application.Current.Dispatcher
            );

            dispatchTimer.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Messenger.Log("Closing J.U.K.E.");
            viewModel.Dispose();
            dispatchTimer.Stop();
        }

        private void CreateFadeAnimation()
        {
            CreateFadeOutAnimation();
            CreateFadeInAnimation();
            CreateEndlessFadeAnimation();
            CreateHideSearchAnimation();
        }

        private void CreateHideSearchAnimation()
        {
            searchHideAnimation = new DoubleAnimation(1, 0.0, new Duration(TimeSpan.FromSeconds(0.5)))
            {
                FillBehavior = FillBehavior.HoldEnd
            };
            searchHideAnimation.Completed += SearchHideAnimation_Completed;
        }

        private void CreateEndlessFadeAnimation()
        {
            endlessFadeAnimation = new DoubleAnimation(0, 0.4, new Duration(TimeSpan.FromSeconds(0.2)))
            {
                AutoReverse = true
            };
            endlessFadeAnimation.Completed += FadeInAnimation_Completed;
        }

        private void CreateFadeInAnimation()
        {
            fadeInAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(2)))
            {
                DecelerationRatio = 0.7,
                FillBehavior = FillBehavior.HoldEnd
            };
        }

        private void CreateFadeOutAnimation()
        {
            fadeOutAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(3)))
            {
                DecelerationRatio = 0.7,
                FillBehavior = FillBehavior.Stop
            };
            fadeOutAnimation.Completed += FadeAnimation_Completed;
        }

        private void SearchHideAnimation_Completed(object sender, EventArgs e)
        {
            searchBox.Opacity = 1;
            searchBox.Visibility = Visibility.Hidden;
            searchBox.BeginAnimation(OpacityProperty, null);
        }

        private void FadeInAnimation_Completed(object sender, EventArgs e)
        {
            if (loaded)
            {
                Messenger.Log("FadeIn /Load completed");
                logoLabel.BeginAnimation(OpacityProperty, fadeInAnimation, HandoffBehavior.SnapshotAndReplace);
                loadProgress.Visibility = Visibility.Collapsed;
            }
            else
            {
                logoLabel.BeginAnimation(OpacityProperty, endlessFadeAnimation, HandoffBehavior.SnapshotAndReplace);
            }

            InvalidateVisual();
        }

        private void FadeAnimation_Completed(object sender, EventArgs e)
        {
            if (viewModel.Queue.Count > 0)
            {
                queueBox.Opacity = 0;
            }

            songLabel.Opacity = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            jukeController = JukeController.Instance;

            jukeController.Player.SongPlayed += Player_SongPlayed;

            if (!File.Exists("library.xml"))
            {
                Messenger.Log("Library doesn't exist. Create new one.");
                CreateLibrary();
            }
            else
            {
                Messenger.Log("Loading existing library");
                LoadLibrary();
            }

            searchLogics = new SearchLogicFactory().CreateAll(viewModel);
            searchLogic = searchLogics[0];
            Messenger.Log("Main window loaded");
        }

        private void CreateLibrary()
        {
            loadProgress.Visibility = Visibility.Hidden;
            var window = new IntroWindow(viewModel);
            windowRouter.ShowDialog(window);
            Messenger.Log("Intro Window closed");
            viewModel.View = this;
            AddHandler(Window.KeyDownEvent, new KeyEventHandler(Window_KeyDown), true);
            viewModel.SaveLibrary.Execute(this);
            Focus();
            loaded = true;
            logoLabel.BeginAnimation(OpacityProperty, fadeInAnimation, HandoffBehavior.SnapshotAndReplace);
        }

        private void LoadLibrary()
        {
            logoLabel.BeginAnimation(OpacityProperty, endlessFadeAnimation, HandoffBehavior.SnapshotAndReplace);
            viewModel.LoadLibrary.Execute(this);
        }

        private void Player_SongPlayed(object sender, Song song)
        {
            AnimateSongTitles();
        }

        private void AnimateSongTitles()
        {
            if (viewModel.Queue.Count > 0)
            {
                RenderQueue();
            }

            if (jukeController.Player.NowPlaying != null)
            {
                RenderPlayingSong();
            }
        }

        private void RenderQueue()
        {
            queueBox.Items.Clear();
            foreach (var s in viewModel.Queue)
            {
                queueBox.Items.Add(s);
            }

            queueBox.Opacity = 100;
            queueBox.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private void RenderPlayingSong()
        {
            songLabel.Content = jukeController.Player.NowPlaying.Name + "\r\n" + jukeController.Player.NowPlaying.Album
                                + "\r\n" + jukeController.Player.NowPlaying.Artist;
            songLabel.Opacity = 100;
            songLabel.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                HandleAdminKeys(e);

                return;
            }

            if (!searchBox.IsVisible)
            {
                RevealSearchBox();
            }
            else
            {
                HandleSearchBoxKeys(e);
            }
        }

        private void HandleAdminKeys(KeyEventArgs e)
        {
            if (e.Key == Key.PageDown)
            {
                var manager = new AdminWindow();
                windowRouter.ShowDialog(manager);
            }

            if (e.Key == Key.PageUp)
            {
                jukeController.Player.PlayRandom();
            }
        }

        private void RevealSearchBox()
        {
            searchBox.Text = "";
            searchBox.Visibility = Visibility.Visible;
            searchBox.Focus();
        }

        private void HandleSearchBoxKeys(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    MoveListCursorUp();
                    return;
                case Key.Down:
                    MoveListCursorDown();
                    return;
                case Key.Enter:
                    PlaySelectedSongFromList();
                    return;
            }
        }

        private void PlaySelectedSongFromList()
        {
            var song = songList.SelectedItem as Song;
            if (!viewModel.PlaySong.CanExecute(null)) return;
            viewModel.PlaySong.Execute(this);

            searchBox.Visibility = Visibility.Hidden;
            songList.Visibility = Visibility.Hidden;
            searchBox.Text = "";
        }

        private void MoveListCursorDown()
        {
            if (songList.SelectedIndex >= songList.Items.Count - 1) return;
            songList.SelectedIndex++;
            songList.ScrollIntoView(songList.SelectedItem);
            viewModel.SelectionTracker.SelectedSong = songList.SelectedItem as Song;
        }

        private void MoveListCursorUp()
        {
            if (songList.SelectedIndex <= 0) return;
            songList.SelectedIndex--;
            songList.ScrollIntoView(songList.SelectedItem);
            viewModel.SelectionTracker.SelectedSong = songList.SelectedItem as Song;
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (songList?.Items == null)
            {
                return;
            }

            songList.Items.Clear();
            if (searchBox.Text.Length > 0)
            {
                AddSongsFromSearch();
                ToggleSongListVisibility();
            }
            else
            {
                ClearSearch();
            }
        }

        private void ToggleSongListVisibility()
        {
            if (songList.Items.Count > 0)
            {
                songList.SelectedIndex = 0;
                songList.Visibility = Visibility.Visible;
            }
            else
            {
                songList.Visibility = Visibility.Hidden;
            }
        }

        private void AddSongsFromSearch()
        {
            var output = searchLogic.Search(searchBox.Text);
            viewModel.SelectionTracker.SelectedSong = output.Count > 0 ? output[0] : null;
            foreach (var song in output)
            {
                songList.Items.Add(song);
            }
        }

        private void ClearSearch()
        {
            viewModel.SelectionTracker.SelectedSong = null;
            searchBox.Text = "";
            searchBox.Visibility = Visibility.Hidden;
            songList.Visibility = Visibility.Hidden;
        }

        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            viewModel.Dispose();
            Environment.Exit(0);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public string PromptPath()
        {
            throw new NotImplementedException();
        }

        public void CommandCompleted(JukeCommand command)
        {
            switch (command)
            {
                case LoadLibraryCommand _:
                    AddHandler(Window.KeyDownEvent, new KeyEventHandler(Window_KeyDown), true);
                    loaded = true;
                    break;
                case SaveLibraryCommand _:
                    Messenger.Log("Library saved!");
                    break;
            }
        }

        private void logoLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ButtonState)
            {
                case MouseButtonState.Pressed when e.ChangedButton == MouseButton.Left:
                    AnimateSongTitles();
                    break;
                case MouseButtonState.Pressed when e.ChangedButton == MouseButton.Right:
                    ShowSearchLogicSelection();
                    break;
            }
        }

        private void ShowSearchLogicSelection()
        {
            var dialog = new SearchSelectorWindow(searchLogics);

            if (dialog.ShowDialog() == true)
            {
                searchLogic = dialog.SearchLogic;
            }
        }

        public SongUpdate PromptSongData(InfoType infoType)
        {
            throw new NotImplementedException();
        }
    }
}