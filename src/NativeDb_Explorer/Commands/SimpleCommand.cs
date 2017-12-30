using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NativeDb_Explorer.Commands
{
    public class SimpleCommand : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; }
        public Action<object> ExecuteDelegate { get; set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
                return CanExecuteDelegate(parameter);
            return true; // if there is no can execute default to true
        }

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
                ExecuteDelegate(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
