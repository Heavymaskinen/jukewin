using Juke.Control;
using System;

namespace Juke.UI.Command
{
    public class DeleteSongCommand : JukeCommand
    {
        public DeleteSongCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return model.SelectedSong != null;
        }

        protected override void ControlledExecute(object parameter)
        {
            Console.WriteLine(model.SelectedSong);
            if (model.SelectedSong == null) return;
            var song = model.SelectedSong;
            Console.WriteLine("Delete: " + song);
            controller.LoadHandler.DeleteSong(song);
        }
    }
}