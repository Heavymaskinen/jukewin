using System.Collections.ObjectModel;
using DataModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Generic;

namespace Juke.UI
{
    public interface SelectionModel : INotifyPropertyChanged
    {
        SelectionTracker SelectionTracker { get; }
        ProgressTracker ProgressTracker { get; }
        ICommand PlaySong {get; }
        IList<Song> SelectedSongs { get; set; }
    }
}