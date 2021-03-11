using System;
using Juke.Control;
using DataModel;

namespace Juke.UI.Command
{
    public class EditSongCommand : JukeCommand
    {
        public EditSongCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            var infoType = (InfoType) parameter;
            Messenger.Log("Editing " + infoType+". Opening data prompt");
            SongUpdate update = view.PromptSongData(infoType);
            Messenger.Log("Prompt closed. "+update);
            if (update != null)
            {
                switch (infoType)
                {
                    case InfoType.Album:
                        controller.LoadHandler.RenameAlbum(update);
                        model.SelectedAlbum = update.NewAlbum;
                        Messenger.Log("Album updated");
                        break;
                    case InfoType.Artist:
                        controller.LoadHandler.RenameArtist(update);
                        model.SelectedArtist = update.NewArtist;
                        Messenger.Log("Artist updated");
                        break;
                    case InfoType.Song:
                        controller.LoadHandler.UpdateSong(update);
                        model.SelectedSong = update.ToSong();
                        Messenger.Log("Song updated");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}