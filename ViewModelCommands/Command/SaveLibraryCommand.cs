using Juke.Control;
using System.Threading.Tasks;

namespace Juke.UI.Command
{
    public class SaveLibraryCommand : JukeCommand
    {
        public SaveLibraryCommand(JukeController controller, ViewControl view, SelectionModel model) : base(controller,
            view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            Task.Run(() =>
            {
                controller.SaveLibrary(new WriterFactory().CreateWriter("library.xml"));
                view.CommandCompleted(this);
            });
        }
    }
}