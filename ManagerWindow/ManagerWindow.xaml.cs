using Juke.External.Wmp.IO;
using Juke.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataModel;
using Juke.External.Lookup;
using System.IO;

namespace Juke.UI.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ViewControl
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new JukeViewModel(this);
            LoaderFactory.SetLoaderInstance(new AsyncFileFinder(""));
            (DataContext as JukeViewModel).PropertyChanged += MainWindow_PropertyChanged;
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
                albumBox.Items.Refresh();
                albumBox.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
            }
            else if (e.PropertyName == "Songs")
            {
                songBox.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
            }
            else if (e.PropertyName == "Artists")
            {
                artistBox.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending));
            }
        }

        public string PromptPath()
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                return dlg.SelectedPath;
            }

            return null;
        }

        public SongUpdate PromptSongData()
        {
            return new SongUpdate(null);
        }
    }
}
