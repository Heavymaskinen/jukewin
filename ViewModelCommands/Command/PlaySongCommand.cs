using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Control;

namespace Juke.UI.Command
{
    public class PlaySongCommand : JukeCommand
    {
        public PlaySongCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
            model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSong")
            {
                SignalCanExecuteChanged(this);
            }
        }

        public override bool CanExecute(object parameter)
        {
            return model.SelectedSong != null;
        }

        protected override void ControlledExecute(object parameter)
        {
            Console.WriteLine("Play it: " + model.SelectedSong + " " + model.SelectedSong.Album + " " +
                              model.SelectedSong.Artist + " " + model.SelectedSong.FilePath);
            controller.Player.PlaySong(model.SelectedSong);
        }
    }
}