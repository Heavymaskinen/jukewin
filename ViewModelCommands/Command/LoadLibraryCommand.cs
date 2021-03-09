using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            await Task.Run(() =>
            {
                controller.LoadHandler.LoadSongsSync(new XmlSongReader("library.xml"));
                Console.WriteLine("Back from loadLibrary");
                Console.WriteLine("SEnd complete to " + view.GetType());
                view.CommandCompleted(this);
            });
            
        }
    }
}
