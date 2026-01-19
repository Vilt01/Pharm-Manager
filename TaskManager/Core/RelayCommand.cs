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
        // 1. ПОЛЯ (Хранилище)
        // Action<object> - это переменная, в которой лежит ТВОЙ метод (например, TryLogin).
        // object - значит метод может принимать любой параметр.
        private readonly Action<object> _execute;

        // Predicate<object> - это переменная для метода-проверки (можно ли нажать кнопку?).
        // Он возвращает true/false.
        private readonly Predicate<object> _canExecute;

        // 2. КОНСТРУКТОР (Загрузка)
        // Когда ты пишешь new RelayCommand(TryLogin), метод TryLogin попадает сюда
        // в переменную execute. И мы сохраняем его в _execute.
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;       // Запомнили метод, который надо выполнить.
            _canExecute = canExecute; // Запомнили проверку (если есть).
        }

        // 3. МЕТОДЫ ИНТЕРФЕЙСА (То, что дергает Кнопка)

        // Кнопка периодически спрашивает: "Я могу быть активной?"
        // Мы запускаем сохраненный метод проверки (_canExecute).
        // Если проверки нет (null), возвращаем true (всегда можно).
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        // Самое главное! Когда юзер жмет кнопку, Кнопка вызывает этот метод.
        // А мы внутри вызываем ТВОЙ метод (_execute), который сохранили в конструкторе.
        public void Execute(object parameter) => _execute(parameter);

        // Это событие нужно, чтобы кнопка узнала, если статус "можно нажать" изменился.
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}