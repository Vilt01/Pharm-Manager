using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TaskManager.Core
{
    // Мы реализуем ICommand, чтобы Кнопка (Button) признала этот класс "своим".
    public class RelayCommand : ICommand
    {
        
        private readonly Action<object> _execute;

        
        private readonly Predicate<object> _canExecute;

 
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;       // Запомнили метод, который надо выполнить.
            _canExecute = canExecute; // Запомнили проверку (если есть).
        }

       
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        
        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}