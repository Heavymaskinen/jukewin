using DataModel;
using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.Command
{
    public class DeleteAlbumCommand : AsyncJukeCommand
    {
        public DeleteAlbumCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return (model.SelectedAlbum != null && model.SelectedAlbum != Song.ALL_ALBUMS);
        }

        protected override Task AsyncExecute(object parameter)
        {
            return Task.Run(() =>
            {
                if (model.SelectedAlbum == null || model.SelectedAlbum == Song.ALL_ALBUMS) return;

                string selectedAlbum = model.SelectedAlbum;
                Messenger.Post("Deleting album: " + selectedAlbum);
                controller.LoadHandler.DeleteAlbum(selectedAlbum, model.ProgressTracker);
                Messenger.Post(selectedAlbum + " was deleted from library");
            });
        }
    }
}
