using Juke.Control;
using System;
using System.Windows.Input;

namespace Juke.UI.Command
{
    public abstract class JukeCommand : ICommand
    {
        protected IJukeController controller;
        protected ViewControl view;
        protected SelectionModel model;

        protected JukeCommand(IJukeController controller, ViewControl view, SelectionModel model)
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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