using DataModel;
using System.ComponentModel;

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
    }
}
