using DataModel;
using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.Command
{
    internal class DeleteAlbumCommand : JukeCommand
    {
        public DeleteAlbumCommand(JukeController controller, ViewControl view, JukeViewModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            if (model.SelectedAlbum == null || model.SelectedAlbum == Song.ALL_ALBUMS) return;

            var songs = controller.Browser.GetSongsByAlbum(model.SelectedAlbum);
            foreach ( var song in songs)
            {
                controller.LoadHandler.DeleteSong(song);
            }
        }
    }
}
