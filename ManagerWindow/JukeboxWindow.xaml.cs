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
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Juke.UI.Wpf
{
    /// <summary>
    /// Interaction logic for JukeboxWindow.xaml
    /// </summary>
    public partial class JukeboxWindow : Window, ViewControl
    {

        private JukeController jukeController;
        private AnimationTimeline fadeOutAnimation;
        private AnimationTimeline fadeInAnimation;
        private AnimationTimeline endlessFadeAnimation;
        private AnimationTimeline borderOutAnimation;
        private AnimationTimeline borderInAnimation;
        private AnimationTimeline searchHideAnimation;
        private JukeViewModel viewModel;
        private bool loaded;

        private DispatcherTimer dispatchTimer;
        private SearchLogic searchLogic;
        private IList<SearchLogic> searchLogics;

        public JukeboxWindow()
        {
            InitializeComponent();
            CreateFadeAnimation();
            borderOutAnimation = new ThicknessAnimation(new Thickness(99.0), new Thickness(0.0), new Duration(TimeSpan.FromSeconds(5)));
            borderOutAnimation.DecelerationRatio = 0.7;
            borderInAnimation = new ThicknessAnimation(new Thickness(0.0), new Thickness(99.0), new Duration(TimeSpan.FromSeconds(5)));
            borderInAnimation.DecelerationRatio = 0.7;
            viewModel = new JukeViewModel(this, new WmpPlayerEngine());
            DataContext = viewModel;
            loaded = false;

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
            viewModel.Dispose();
            dispatchTimer.Stop();
        }

        private void ComponentDispatcher_ThreadIdle(object sender, EventArgs e)
        {

        }

        private void CreateFadeAnimation()
        {
            fadeOutAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(3)))
            {
                DecelerationRatio = 0.7,
                FillBehavior = FillBehavior.Stop
            };
            fadeOutAnimation.Completed += FadeAnimation_Completed;

            fadeInAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(2)))
            {
                DecelerationRatio = 0.7,
                FillBehavior = FillBehavior.HoldEnd
            };

            endlessFadeAnimation = new DoubleAnimation(0, 0.4, new Duration(TimeSpan.FromSeconds(0.5)))
            {
                AutoReverse = true
            };
            endlessFadeAnimation.Completed += FadeInAnimation_Completed;

            searchHideAnimation = new DoubleAnimation(1, 0.0, new Duration(TimeSpan.FromSeconds(0.5)))
            {
                FillBehavior = FillBehavior.HoldEnd
            };
            searchHideAnimation.Completed += SearchHideAnimation_Completed;

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
            if (jukeController.Player.Queue.Count > 0)
            {
                queueBox.Opacity = 0;

            }

            songLabel.Opacity = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            jukeController = JukeController.Instance;
            jukeController.Player.RegisterPlayerEngine(new WmpPlayerEngine());
            jukeController.LoadHandler.LibraryUpdated += LoadHandler_LibraryUpdated;
            Player.SongPlayed += Player_SongPlayed;

            if (!File.Exists("library.xml"))
            {
                loadProgress.Visibility = Visibility.Hidden;
                var window = new IntroWindow(viewModel);
                Visibility = Visibility.Hidden;
                window.ShowDialog();
                Visibility = Visibility.Visible;
                viewModel.View = this;
                AddHandler(Window.KeyDownEvent, new KeyEventHandler(Window_KeyDown), true);
                viewModel.SaveLibrary.Execute(this);
                loaded = true;
                logoLabel.BeginAnimation(OpacityProperty, fadeInAnimation, HandoffBehavior.SnapshotAndReplace);
            }
            else
            {
                viewModel.LoadLibrary.Execute(this);
                logoLabel.BeginAnimation(OpacityProperty, endlessFadeAnimation, HandoffBehavior.SnapshotAndReplace);
            }

            ComponentDispatcher.ThreadIdle += ComponentDispatcher_ThreadIdle;
            searchLogics = new SearchLogicFactory().CreateAll(viewModel);
            searchLogic = searchLogics[0];
        }

        private void LoadHandler_LibraryUpdated(object sender, EventArgs e)
        {


        }

        private void Player_SongPlayed(object sender, Song song)
        {
            AnimateSongTitles();
        }

        private void AnimateSongTitles()
        {
            if (jukeController.Player.Queue.Count > 0)
            {
                queueBox.Items.Clear();
                foreach (var s in jukeController.Player.Queue.Songs)
                {
                    queueBox.Items.Add(s);
                }
                queueBox.Opacity = 100;
                queueBox.BeginAnimation(OpacityProperty, fadeOutAnimation);
            }

            if (jukeController.Player.NowPlaying != null)
            {
                songLabel.Content = jukeController.Player.NowPlaying.Name + "\r\n" + jukeController.Player.NowPlaying.Album
                    + "\r\n" + jukeController.Player.NowPlaying.Artist;
                songLabel.Opacity = 100;
                songLabel.BeginAnimation(OpacityProperty, fadeOutAnimation);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Key == Key.PageDown)
                {
                    var manager = new MainWindow();
                    manager.ShowDialog();
                }

                if (e.Key == Key.PageUp)
                {
                    jukeController.Player.PlayRandom();
                }

                return;
            }
            if (!searchBox.IsVisible)
            {
                searchBox.Text = "";
                searchBox.Visibility = Visibility.Visible;
                searchBox.Focus();
            }
            else
            {
                if (e.Key == Key.Up)
                {
                    if (songList.SelectedIndex > 0)
                    {
                        songList.SelectedIndex--;
                        songList.ScrollIntoView(songList.SelectedItem);
                    }

                    return;
                }

                if (e.Key == Key.Down)
                {
                    if (songList.SelectedIndex < songList.Items.Count - 1)
                    {
                        songList.SelectedIndex++;
                        songList.ScrollIntoView(songList.SelectedItem);
                    }

                    return;
                }

                if (e.Key == Key.Enter)
                {
                    Song song = songList.SelectedItem as Song;
                    jukeController.Player.PlaySong(song);
                    searchBox.Visibility = Visibility.Hidden;
                    songList.Visibility = Visibility.Hidden;
                    searchBox.Text = "";
                    return;
                }

            }
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (songList == null || songList.Items == null)
            {
                return;
            }

            songList.Items.Clear();
            if (searchBox.Text.Length > 0)
            {
                var output = searchLogic.Search(searchBox.Text);
                output.Sort(Song.Comparison);

                foreach (var song in output)
                {
                    songList.Items.Add(song);
                }

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
            else
            {
                searchBox.Text = "";
                searchBox.Visibility = Visibility.Hidden;
                songList.Visibility = Visibility.Hidden;
            }
        }

        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
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

        public SongUpdate PromptSongData()
        {
            throw new NotImplementedException();
        }

        public void CommandCompleted(JukeCommand command)
        {
            if (command is LoadLibraryCommand)
            {
                AddHandler(Window.KeyDownEvent, new KeyEventHandler(Window_KeyDown), true);
                loaded = true;
            }
            else if (command is SaveLibraryCommand)
            {
                Console.WriteLine("Library saved!");
            }

        }

        private void logoLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
            {
                AnimateSongTitles();
            }
            else if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Right)
            {
                var dialog = new SearchSelectorWindow(searchLogics);

                if (dialog.ShowDialog() == true)
                {
                    searchLogic = dialog.SearchLogic;
                }
            }
        }

        public SongUpdate PromptSongData(JukeViewModel.InfoType infoType)
        {
            throw new NotImplementedException();
        }
    }
}
