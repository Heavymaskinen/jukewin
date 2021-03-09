using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.Command
{
    public class AsyncLoadSongsCommand : AsyncJukeCommand
    {
        public AsyncLoadSongsCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override Task AsyncExecute(object parameter)
        {
            return Task.Run(() =>
            {
                var path = view.PromptPath();
                if (!string.IsNullOrEmpty(path))
                {
                    var loader = new LoaderFactory().CreateAsync(path);
                    controller.LoadHandler.LoadSongs(loader);
                }
            });
        }
    }
}
