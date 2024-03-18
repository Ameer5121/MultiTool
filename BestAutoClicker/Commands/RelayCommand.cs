using BestAutoClicker.Helper.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BestAutoClicker.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Action<AutoClickerMode> execute2;
        private readonly Action<ClickingMode> execute3;
        private readonly Func<bool> canExecute;
        public RelayCommand(Action execute) : this(execute, canExecute: null)
        {
        }
        public RelayCommand(Action<AutoClickerMode> execute2) : this(execute2, canExecute: null)
        {
        }
        public RelayCommand(Action<ClickingMode> execute3) : this(execute3, canExecute: null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute is null");

            this.execute = execute;
            this.canExecute = canExecute;
        }
        public RelayCommand(Action<AutoClickerMode> execute2, Func<bool> canExecute)
        {
            if (execute2 == null)
                throw new ArgumentNullException("execute2 is null");

            this.execute2 = execute2;
            this.canExecute = canExecute;
        }
        public RelayCommand(Action<ClickingMode> execute3, Func<bool> canExecute)
        {
            if (execute3 == null)
                throw new ArgumentNullException("execute3 is null");

            this.execute3 = execute3;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }


        public bool CanExecute(object parameter)
        {
            return this.canExecute == null ? true : this.canExecute();
        }

        public async void Execute(object parameter)
        {
            if (execute != null)
            {
                execute();
            }
            else if (execute2 != null) execute2((AutoClickerMode)parameter);
            else if (execute3 != null) execute3((ClickingMode)parameter);
        }
    }
}
