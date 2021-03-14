﻿using System;
using System.Threading.Tasks;
using Juke.Control;
using Juke.External.Xml;

namespace Juke.UI.Command
{
    public class LoadLibraryCommand : AsyncJukeCommand
    {
        public LoadLibraryCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override Task AsyncExecute(object parameter)
        {
            return Task.Run(async () =>
            {
                await controller.LoadHandler.LoadSongs(new XmlSongReader("library.xml"), model.ProgressTracker);
                Messenger.Log("Done loading library");
                view.CommandCompleted(this);
            });
        }
    }
}