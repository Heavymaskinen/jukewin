using System;
using System.Threading.Tasks;
using Juke.Control;
using Juke.External.Xml;

namespace Juke.UI.Command
{
    public class LoadLibraryCommand : JukeCommand
    {
        public LoadLibraryCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            AsyncExecute();
        }

        private async void AsyncExecute()
        {
            await controller.LoadHandler.LoadSongs(new XmlSongReader("library.xml"), model.ProgressTracker);
            Messenger.Log("Done loading library");
            view.CommandCompleted(this);
        }
    }
}
