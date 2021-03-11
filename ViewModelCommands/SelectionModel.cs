using System.Collections.ObjectModel;
using DataModel;
using System.ComponentModel;
using System.Windows.Input;
using UiComponents;
using System.Collections.Generic;

namespace Juke.UI
{
    public interface SelectionModel : INotifyPropertyChanged
    {
        string SelectedArtist { get; set; }

        string SelectedAlbum { get; set; }

        Song SelectedSong { get; set; }

        ProgressTracker ProgressTracker { get; }
        ObservableCollection<Song> Queue { get; }
        ObservableCollection<Song> Songs { get;}
        ObservableCollection<string> Albums { get; }
        ObservableCollection<string> Artists { get; }
        ICommand PlaySong {get; }
        IList<Song> SelectedSongs { get; set; }
    }
}