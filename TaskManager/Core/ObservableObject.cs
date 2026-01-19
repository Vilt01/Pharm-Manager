using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Core
{
    // 1. Мы реализуем интерфейс INotifyPropertyChanged.
    // Это значит, мы ОБЯЗАНЫ создать событие PropertyChanged.
    public class ObservableObject : INotifyPropertyChanged
    {
        // 2. Вот оно. Это само событие.
        // WPF "подписывается" на него. Когда оно сработает, WPF перерисует экран.
        public event PropertyChangedEventHandler PropertyChanged;

        // 3. Это вспомогательный метод, чтобы нам было удобно запускать событие.
        // [CallerMemberName] - это магия компилятора.
        // Если ты вызовешь этот метод внутри свойства Username, компилятор сам
        // подставит строку "Username" вместо name. Тебе не надо писать это вручную.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            // 4. Invoke - это команда "Запуск!".
            // Знак вопроса "?" - проверка на null. Если никто не подписался (WPF не смотрит), то ничего не делаем.
            // this - кто отправил (мы сами).
            // new PropertyChangedEventArgs(name) - посылка с названием изменившегося свойства.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
