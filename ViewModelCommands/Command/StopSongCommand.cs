﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Control;

namespace Juke.UI.Command
{
    public class StopSongCommand : JukeCommand
    {

        public StopSongCommand(IJukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
            this.controller = controller;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            controller.Player.Stop();
        }
    }
}