using DataModel;
using System.ComponentModel;
using UiComponents;

namespace Juke.UI
{
    public interface SelectionModel : INotifyPropertyChanged
    {
        string SelectedArtist
        {
            get;set;
        }

        string SelectedAlbum
        {
            get;set;
        }

        Song SelectedSong
        {
            get;set;
        }

        ProgressTracker ProgressTracker { get; }
    }
}
