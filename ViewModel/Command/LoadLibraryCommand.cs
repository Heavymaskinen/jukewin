using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Control;
using Juke.External.Xml;

namespace Juke.UI.Command
{
    class LoadLibraryCommand : JukeCommand
    {
        public LoadLibraryCommand(JukeController controller, ViewControl view, JukeViewModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            controller.LoadLibrary(new XmlSongReader("library.xml"));
        }
    }
}
