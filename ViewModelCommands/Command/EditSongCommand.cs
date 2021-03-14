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
                        update.NewName = null;
                        controller.LoadHandler.RenameAlbum(update);
                        model.SelectionTracker.SelectedAlbum = update.NewAlbum;
                        model.SelectionTracker.SelectedArtist = update.ToSong().Artist;
                        Messenger.Log("Album updated");
                        break;
                    case InfoType.Artist:
                        update.NewName = null;
                        update.NewAlbum = null;
                        controller.LoadHandler.RenameArtist(update);
                        model.SelectionTracker.SelectedArtist = update.NewArtist;
                        Messenger.Log("Artist updated");
                        break;
                    case InfoType.Song:
                        controller.LoadHandler.UpdateSong(update);
                        var updatedSong = update.ToSong();
                        model.SelectionTracker.SelectedSong = updatedSong;
                        Messenger.Log("Song updated to: "+updatedSong.Artist+" "+updatedSong.Album+" "+updatedSong.Name);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}