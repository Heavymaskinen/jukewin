using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Control;
using Juke.UI;
using Juke.UI.Command;

namespace ViewModelCommands.Command
{
    public class SkipSongCommand : JukeCommand
    {
        public SkipSongCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            controller.Player.Skip();
        }
    }
}
