using Juke.Control;
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
            return model.SelectionTracker.SelectedSongs?.Count > 0;
        }

        protected override Task AsyncExecute(object parameter)
        {
            return Task.Run(() =>
            {
                controller.LoadHandler.DeleteSongs(model.SelectionTracker.SelectedSongs, model.ProgressTracker);
            });
        }
    }
}