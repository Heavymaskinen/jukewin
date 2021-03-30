using System;
using System.Threading.Tasks;
using Juke.Control;

namespace Juke.UI.Command
{
    public class LoadLibraryCommand : AsyncJukeCommand
    {
        public LoadLibraryCommand(IJukeController controller, ViewControl view, SelectionModel model) : base(controller,
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
                var loader = new LoaderFactory().CreateLibraryLoader("library.xml");
                await controller.LoadHandler.LoadSongs(loader, model.ProgressTracker);
                Messenger.Log("Done loading library");
                view.CommandCompleted(this);
            });
        }
    }
}