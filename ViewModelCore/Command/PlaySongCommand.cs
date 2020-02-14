using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Control;

namespace Juke.UI.Command
{
    class PlaySongCommand : JukeCommand
    {
        public PlaySongCommand(IJukeControl controller, ViewControl view, JukeViewModel model) : base(controller, view, model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSong")
            {
                SignalCanExecuteChanged(this);
            }
        }

        public override bool CanExecute(object parameter)
        {
            return model.SelectedSong != null;
        }

        protected override void ControlledExecute(object parameter)
        {
            controller.Player.PlaySong(model.SelectedSong);
        }
    }
}
