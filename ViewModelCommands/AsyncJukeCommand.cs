using Juke.Control;
using Juke.UI.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Juke.UI
{
    public abstract class AsyncJukeCommand : JukeCommand
    {
        public event EventHandler CanExecuteChanged;

        protected JukeController controller;
        protected ViewControl view;
        protected SelectionModel model;

        protected AsyncJukeCommand(JukeController controller, ViewControl view, SelectionModel model):base(controller, view, model)
        {
            this.controller = controller;
            this.view = view;
            this.model = model;
        }

        protected override void ControlledExecute(object parameter)
        {
            _ = RunAsync(AsyncExecute(parameter)).ConfigureAwait(false);
        }

        private async Task RunAsync(Task task)
        {
            await task.ConfigureAwait(false);
        }

        protected abstract Task AsyncExecute(object parameter);

        protected void SignalCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}