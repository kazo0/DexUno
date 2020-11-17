using System;
using System.Windows.Input;

namespace Dex.Uwp.Infrastructure
{
    public class RelayCommand : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Action execute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute ?? (() => true);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter = null)
        {
            return canExecute();
        }

        public void Execute(object parameter)
        {
            if (canExecute())
                execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

	public class RelayCommand<T> : ICommand where T : class
	{
        private readonly Func<bool> canExecute;
        private readonly Action<T> execute;

        public RelayCommand(Action<T> execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute ?? (() => true);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter = null)
        {
            return canExecute();
        }

        public void Execute(object parameter)
        {
            if (canExecute())
                execute(parameter as T);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}