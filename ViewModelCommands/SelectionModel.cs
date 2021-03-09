using DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
