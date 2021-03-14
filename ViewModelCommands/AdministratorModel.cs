using System.Windows.Input;
using Juke.UI;

namespace ViewModelCommands
{
    public interface AdministratorModel : SelectionModel
    {
        ICommand LoadSongs { get; }
        ICommand LoadLibrary { get; }
        ICommand SaveLibrary { get; }
        ICommand StopSong { get; }
        ICommand SkipSong { get; }
        ICommand EditSong { get; } 
        ICommand EditAlbum { get; }
        ICommand RenameArtist { get; }
        ICommand DeleteAlbum { get; }
        ICommand DeleteSong { get; }
    }
}