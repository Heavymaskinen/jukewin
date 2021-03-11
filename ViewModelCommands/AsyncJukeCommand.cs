using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Juke.UI
{
    public abstract class AsyncJukeCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        protected JukeController controller;
        protected ViewControl view;
        protected SelectionModel model;

        protected AsyncJukeCommand(JukeController controller, ViewControl view, SelectionModel model)
        {
            this.controller = controller;
            this.view = view;
            this.model = model;
        }

        public abstract bool CanExecute(object parameter);

        public void Execute(object parameter)
        {
            try
            {
                _ = RunAsync(AsyncExecute(parameter)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Messenger.Log(e.Message);
                Messenger.Log(e.InnerException.Message);
                Messenger.Log(e.ToString());
            }
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