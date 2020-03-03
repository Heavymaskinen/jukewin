using Juke.External.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Control;

namespace Juke.UI.Command
{
    class SaveLibraryCommand : JukeCommand
    {
        public SaveLibraryCommand(JukeController controller, ViewControl view, JukeViewModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            controller.SaveLibrary(new XmlSongWriter("library.xml"));
        }
    }
}
