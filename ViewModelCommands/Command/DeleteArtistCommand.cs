using System.Threading.Tasks;
using DataModel;
using Juke.Control;

namespace Juke.UI.Command
{
    public class DeleteArtistCommand : AsyncJukeCommand
    {
        public DeleteArtistCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return model.SelectedArtist != null && model.SelectedArtist != Song.ALL_ARTISTS &&
                   !LoaderCancellationTokenProvider.Token.CanBeCanceled;
        }

        protected override Task AsyncExecute(object parameter)
        {
            return Task.Run(() =>
            {
                var selectedArtist = model.SelectedArtist;
                Messenger.Post("Deleting artist: " + selectedArtist);
                controller.LoadHandler.DeleteArtist(selectedArtist, model.ProgressTracker);
                Messenger.Post(selectedArtist + " was deleted.");
            });
        }
    }
}