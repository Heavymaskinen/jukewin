using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.Command
{
    internal class DeleteSongCommand : JukeCommand
    {
        public DeleteSongCommand(JukeController controller, ViewControl view, JukeViewModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            if (model.SelectedSong == null) return;
            controller.LoadHandler.DeleteSong(model.SelectedSong);
        }
    }
}
