using System.Threading.Tasks;
using DataModel;
using Juke.Control;

namespace Juke.UI.Command
{
    public class DeleteArtistCommand : AsyncJukeCommand
    {
        public DeleteArtistCommand(IJukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedArtist")
            {
                SignalCanExecuteChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return model.SelectionTracker.SelectedArtist != Song.ALL_ARTISTS;
        }

        protected override Task AsyncExecute(object parameter)
        {
            return Task.Run(() =>
            {
                var selectedArtist = model.SelectionTracker.SelectedArtist;
                Messenger.Post("Deleting artist: " + selectedArtist);
                controller.LoadHandler.DeleteArtist(selectedArtist, model.ProgressTracker);
                Messenger.Post(selectedArtist + " was deleted.");
            });
        }
    }
}