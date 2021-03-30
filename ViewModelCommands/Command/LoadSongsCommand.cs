using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Juke.Control;
using Juke.IO;

namespace Juke.UI.Command
{
    public class LoadSongsCommand : JukeCommand
    {
        public LoadSongsCommand(IJukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            var path = view.PromptPath();
            if (!string.IsNullOrEmpty(path))
            {
                LoadAsync(path);
            }
        }

        private void LoadAsync(string path)
        {
            Messenger.Log("Load command with " + path+"|");
            var loader = new LoaderFactory().CreateAsync(path);
            controller.LoadHandler.LoadSongs(loader, model.ProgressTracker);
        }
    }
}