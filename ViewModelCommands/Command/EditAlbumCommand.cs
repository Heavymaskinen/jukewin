using DataModel;
using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.Command
{
    public class EditAlbumCommand : JukeCommand
    {
        public EditAlbumCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            var updateData = view.PromptSongData(InfoType.Album);
            if (updateData == null) return;

            var albumSongs = controller.Browser.GetSongsByAlbum(updateData.SongSource.Album);
            foreach (var song in albumSongs)
            {
                var update = new SongUpdate(song) {NewAlbum = updateData.NewAlbum, NewArtist = updateData.NewArtist};
                controller.LoadHandler.UpdateSong(update);
            }
        }
    }
}