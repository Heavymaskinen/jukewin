using DataModel;
using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.Command
{
    internal class RenameArtistCommand : JukeCommand
    {
        public RenameArtistCommand(JukeController controller, ViewControl view, JukeViewModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            var updateData = view.PromptSongData(JukeViewModel.InfoType.Artist);
            if (updateData == null || updateData.NewArtist == updateData.SongSource.Artist) return;
            var songs = controller.Browser.GetSongsByArtist(updateData.SongSource.Artist);
            foreach (var song in songs)
            {
                var update = new SongUpdate(song) { NewArtist = updateData.NewArtist };
                controller.LoadHandler.UpdateSong(update);
            }

        }
    }
}
