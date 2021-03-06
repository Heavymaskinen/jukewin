﻿using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.Command
{
    public class PlayAlbumCommand : JukeCommand
    {
        public PlayAlbumCommand(IJukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is string && model.SelectionTracker.SelectedAlbum.Equals(parameter);
        }

        protected override void ControlledExecute(object parameter)
        {
            controller.Player.PlayAlbum(model.SelectionTracker.SelectedAlbum);
        }
    }
}