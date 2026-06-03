using TaskManager.Core;

namespace TaskManager.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        // === 1. ТЕКУЩИЙ ВИД (Что показываем справа) ===
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        // === 2. КОМАНДЫ НАВИГАЦИИ ===
        public RelayCommand NavigateToHomeCommand { get; }
        public RelayCommand NavigateToRequestsCommand { get; }
        public RelayCommand NavigateToUserCommand { get; }
        public RelayCommand NavigateToSettingCommand { get; }

        // === 3. КОНСТРУКТОР ===
        public MainViewModel()
        {
            // Настройка кнопок
            NavigateToHomeCommand = new RelayCommand(o =>
            {
                CurrentView = new AnalyticsViewModel();
            });

            NavigateToRequestsCommand = new RelayCommand(o =>
            {
                // Вот тут мы создаем тот самый мощный RequestsViewModel, который писали ранее
                CurrentView = new ZaprosViewModels();
            });

            NavigateToUserCommand = new RelayCommand(o =>
            {
                CurrentView = new UsersViewModel();
            });

            NavigateToSettingCommand = new RelayCommand(o =>
            {
                CurrentView = "В разработке";
            });

        // По умолчанию открываем "Главную"
        CurrentView = new AnalyticsViewModel();
        }
    }
}