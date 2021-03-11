using DataModel;
using Juke.Control;
using System.Threading.Tasks;

namespace Juke.UI.Command
{
    public class DeleteAlbumCommand : AsyncJukeCommand
    {
        public DeleteAlbumCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedAlbum")
            {
                SignalCanExecuteChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return model.SelectedAlbum != Song.ALL_ALBUMS;
        }

        protected override Task AsyncExecute(object parameter)
        {
            return Task.Run(() =>
            {
                if (model.SelectedAlbum == null || model.SelectedAlbum == Song.ALL_ALBUMS) return;

                var selectedAlbum = model.SelectedAlbum;
                Messenger.Post("Deleting album: " + selectedAlbum);
                controller.LoadHandler.DeleteAlbum(selectedAlbum, model.ProgressTracker);
                Messenger.Post(selectedAlbum + " was deleted from library");
            });
        }
    }
}