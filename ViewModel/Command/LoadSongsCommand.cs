using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Control;

namespace Juke.UI.Command
{
    class LoadSongsCommand : JukeCommand
    {
        public LoadSongsCommand(JukeController controller, ViewControl view, JukeViewModel model) : base(controller, view, model)
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
                var loader = new LoaderFactory().CreateAsync(path);
                controller.LoadHandler.LoadSongs(loader);
            }
            
        }
    }
}
