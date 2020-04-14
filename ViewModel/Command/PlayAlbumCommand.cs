using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.Command
{
    internal class PlayAlbumCommand : JukeCommand
    {
        public PlayAlbumCommand(JukeController controller, ViewControl view, JukeViewModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is string && model.Albums.Contains(parameter.ToString());
        }

        protected override void ControlledExecute(object parameter)
        {
            var albumName = parameter.ToString();
            model.SelectedAlbum = albumName;
            foreach (var s in model.Songs)
            {
                controller.Player.PlaySong(s);
            }
            
        }
    }
}
