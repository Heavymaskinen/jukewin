using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Juke.UI.Command
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object, bool> canExecuteCallback;
        private readonly Action<object> command;
        private bool canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Func<object, bool> canExecuteCallback, Action<object> command)
        {
            this.canExecuteCallback = canExecuteCallback;
            this.command = command;
            canExecute = true;
        }

        public bool CanExecute(object parameter)
        {
            var val = canExecuteCallback(parameter);
            if (val != canExecute)
            {
                canExecute = val;
                CanExecuteChanged?.Invoke(this,EventArgs.Empty);
            }

            return val;
        }

        public void Execute(object parameter)
        {
            command(parameter);
        }
    }
}
