using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Juke.UI.Wpf
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {

        public SongUpdate SongData { get; private set; }
        public InfoWindow()
        {
            InitializeComponent();
        }

        public InfoWindow(SongUpdate song, JukeViewModel.InfoType info)
        {
            InitializeComponent();
            DataContext = song;

            if (info == JukeViewModel.InfoType.Album)
            {
                songBox.IsEnabled = false;
            }

            if (info == JukeViewModel.InfoType.Artist)
            {
                songBox.IsEnabled = false;
                albumBox.IsEnabled = false;
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            SongData = DataContext as SongUpdate;
            DialogResult = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
