using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Control;
using DataModel;

namespace Juke.UI.Command
{
    class EditSongCommand : JukeCommand
    {
        public EditSongCommand(JukeController controller, ViewControl view, JukeViewModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            SongUpdate update = view.PromptSongData(JukeViewModel.InfoType.Song);
            if (update != null)
            {
                controller.LoadHandler.UpdateSong(update);
                model.SelectedAlbum = update.NewAlbum;
            }
        }
    }
}
