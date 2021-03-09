using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using DataModel;
using System.IO;
using Juke.Control;
using Juke.External.Wmp;
using Juke.UI.Command;
using Juke.IO;
using Juke.UI.Admin;

namespace Juke.UI.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ViewControl
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new AdminViewModel(this);
            var wmpTagReaderFactory = new TaglibTagReaderFactory() { BackupFactory = new WmpTagReaderFactory()};
            //LoaderFactory.SetLoaderInstance(new AsyncSongLoader(new FileFinderEngine(), wmpTagReaderFactory));
            (DataContext as AdminViewModel).PropertyChanged += MainWindow_PropertyChanged;
        }

        public MainWindow(AdminViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            (DataContext as AdminViewModel).PropertyChanged += MainWindow_PropertyChanged;
        }

        private static BitmapImage CreateBitmapFromBytes(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            stream.Seek(0, SeekOrigin.Begin);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
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
                Messenger.Log("Path selected: "+ dlg.SelectedPath);
                return dlg.SelectedPath;
            }

            return null;
        }
        
        public SongUpdate PromptSongData(InfoType infoType)
        {
            Song song;

            if (songBox.SelectedItem != null)
            {
                song = songBox.SelectedItem as Song;
            }
            else if (artistBox.SelectedItem != null)
            {
                song = CreateSongDataFromAlbumAndArtist();
            }
            else
            {
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
            if (result == true)
            {
                return dialog.SongData;
            }

            return null;
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
            Song song;
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

        }

    }
}
