using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Juke.UI.Command
{
    abstract class JukeCommand : ICommand
    {
        protected IJukeControl controller;
        protected ViewControl view;
        protected JukeViewModel model;

        protected JukeCommand(IJukeControl controller, ViewControl view, JukeViewModel model)
        {
            this.controller = controller;
            this.view = view;
            this.model = model;
        }

        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);

        public void Execute(object parameter)
        {
            try
            {
                ControlledExecute(parameter);
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException.Message);
                Console.WriteLine(e.ToString());
            }
        }

        protected abstract void ControlledExecute(object parameter);

        protected void SignalCanExecuteChanged(JukeCommand origin)
        {
            CanExecuteChanged?.Invoke(origin, EventArgs.Empty);
        }
    }
}
