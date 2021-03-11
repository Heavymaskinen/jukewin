﻿using Juke.Control;
using System.Threading.Tasks;

namespace Juke.UI.Command
{
    public class DeleteSongCommand : AsyncJukeCommand
    {
        public DeleteSongCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ( e.PropertyName == "SelectedSong")
            {
                SignalCanExecuteChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return model.SelectedSongs?.Count > 0;
        }

        protected void ControlledExecute(object parameter)
        {
            
        }

        protected override Task AsyncExecute(object parameter)
        {
            return Task.Run(() =>
            {
                if (model.SelectedSong == null) return;
                controller.LoadHandler.DeleteSongs(model.SelectedSongs, model.ProgressTracker);
            });
        }
    }
}