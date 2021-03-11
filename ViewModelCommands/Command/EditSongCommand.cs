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
            SongUpdate update = view.PromptSongData(infoType);
            if (update != null)
            {
                controller.LoadHandler.UpdateSong(update);
                model.SelectedAlbum = update.NewAlbum;
            }
        }
    }
}