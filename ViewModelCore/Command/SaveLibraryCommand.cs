using Juke.External.Xml;
using Juke.Control;

namespace Juke.UI.Command
{
    class SaveLibraryCommand : JukeCommand
    {
        public SaveLibraryCommand(IJukeControl controller, ViewControl view, JukeViewModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            controller.SaveHandler.SaveSongs(new XmlSongWriter("library.xml"));
        }
    }
}
