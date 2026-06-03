using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TaskManager.Core;
using TaskManager.Data;
using TaskManager.Models;
using System.Collections.Generic;

namespace TaskManager.ViewModels
{
    public class ZaprosViewModels : ObservableObject
    {
        private readonly AppDbContext _context;

        public ObservableCollection<Zapros> Requests { get; set; } = new ObservableCollection<Zapros>();
        public ICollectionView RequestsView { get; set; }

        //Архив хранения копий всех данных
        private List<Zapros> _allRequests = new List<Zapros>();

        // 2. Список слов для выпадающего меню
        public List<string> Statuses { get; } = new List<string>
        {
            "Все",
            "Создан",
            "В работе",
            "Завершен"
        };

        // === ПОИСК ===
        private string _selectedStatus = "Все";
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
               _selectedStatus = value;
                OnPropertyChanged();

                ApplyFilter();
            }
        }

        private DateTime? _selectedate;
        public DateTime? SelectedDate
        {
            get => _selectedate;
            set
            {
                _selectedate = value;
                OnPropertyChanged();

                ApplyFilter();
            }

        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();

                ApplyFilter();
            }
        }

        // === КОМАНДЫ ===
        public RelayCommand CreateCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand UpdateButtonsStateCommand { get; }

        // === КОНСТРУКТОР ===
        public ZaprosViewModels()
        {
            _context = new AppDbContext();

            CreateCommand = new RelayCommand(o => CreateRequest());
            EditCommand = new RelayCommand(o => EditRequest(), o => CanEdit());
            DeleteCommand = new RelayCommand(o => DeleteRequest(), o => CanDelete());

            // ИСПРАВЛЕНИЕ ОШИБКИ ЗДЕСЬ
            UpdateButtonsStateCommand = new RelayCommand(o =>
            {
                // Вместо EditCommand.RaiseCanExecuteChanged() пишем это:
                // Это заставляет WPF перепроверить все кнопки (CanExecute)
                CommandManager.InvalidateRequerySuggested();
            });

            LoadData();
        }

        private void LoadData()
        {
            Requests.Clear();
            var data = _context.Zapros.OrderByDescending(r => r.Id).ToList();
            _allRequests = data;

            foreach (var item in data)
            {
                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Zapros.IsSelected))
                        UpdateButtonsStateCommand.Execute(null);
                };
                Requests.Add(item);
            }

            RequestsView = CollectionViewSource.GetDefaultView(Requests);
        }

       

        // === ЛОГИКА БЛОКИРОВКИ ===
        private bool CanEdit()
        {
            var selected = Requests.Where(r => r.IsSelected).ToList();
            return selected.Count == 1 && selected[0].StatusRequest == "Создан";
        }

        private bool CanDelete()
        {
            var selected = Requests.Where(r => r.IsSelected).ToList();
            if (selected.Count == 0) return false;
            return selected.All(r => r.StatusRequest == "Создан");
        }

        // === МЕТОДЫ ===
        private void CreateRequest()
        {
            
            // 1. Создаем VM
            var vm = new CreateZaprosViewModels();

            // 2. Настраиваем закрытие
            vm.RequestClose += () =>
            {
                IsModalOpen = false;      // Выключаем флаг
                CreateRequestVM = null;   // Очищаем память
                LoadData();               // Обновляем таблицу
            };

            // 3. ОТКРЫВАЕМ ОКНО (Ты пропустил эту часть)
            CreateRequestVM = vm; // Кладем VM в свойство
            IsModalOpen = true;   // Включаем рубильник
        }

        private void EditRequest()
        {
            var item = Requests.First(r => r.IsSelected);
            MessageBox.Show($"Редактировать ID: {item.Id}");
        }

        private void DeleteRequest()
        {
            // 1. Собираем список выделенных заявок
            var itemsToDelete = Requests.Where(r => r.IsSelected).ToList();

            if (itemsToDelete.Count == 0) return;

            // 2. Спрашиваем подтверждение у пользователя
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить {itemsToDelete.Count} заявок?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // 3. Пытаемся удалить через Entity Framework
                    _context.Zapros.RemoveRange(itemsToDelete);
                    _context.SaveChanges(); // <-- Здесь произойдет попытка записи в БД

                    // 4. Если ошибок не было — обновляем список на экране
                    LoadData();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    // 5. ЛОВИМ ОШИБКУ БАЗЫ ДАННЫХ (Например, если есть записи в Истории)

                    MessageBox.Show(
                        "Невозможно удалить одну или несколько заявок.\n\n" +
                        "Причина: Эти заявки используются в других таблицах (например, в Журнале действий/Истории).\n" +
                        "База данных запрещает удаление для сохранения целостности.",
                        "Ошибка удаления",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    // 6. ОТКАТ (ВАЖНО!)
                    // Если мы не сбросим состояние, программа будет думать, что мы всё ещё хотим удалить эти строки.
                    // Очищаем "ChangeTracker" — список изменений в памяти.
                    _context.ChangeTracker.Clear();

                    // Перезагружаем данные из базы, чтобы вернуть всё как было
                    LoadData();
                }
                catch (Exception ex)
                {
                    // Ловим любые другие непредвиденные ошибки
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

                                //Формирование самой заявки
        //Поле для хранения
         private CreateZaprosViewModels _createRequestVM;
        
       
        public CreateZaprosViewModels CreateRequestVM
        {
            get { return _createRequestVM; } //чтобы xaml мог читать
            set
            {
                _createRequestVM = value; // чтобы запомнить значение
                OnPropertyChanged(); // чтобы увидомить интрефейс
            }
        }
            // Поле для флага видимости
        private bool _isModalOpen;

        // Свойство для флага видимости
        public bool IsModalOpen
        {
            get { return _isModalOpen; }
            set
            {
                _isModalOpen = value;
                OnPropertyChanged();
            }
        }

        private void ApplyFilter()
        {
            // 1. Берем исходник со склада
            var filtered = _allRequests.AsEnumerable();

            // 2. Проверяем ПОИСК (если там что-то написано)
            if (!string.IsNullOrEmpty(SearchText))
            {
                filtered = filtered.Where(r => r.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // 3. Проверяем СТАТУС (если выбран конкретный)
            if (!string.IsNullOrEmpty(SelectedStatus) && SelectedStatus != "Все")
            {
                filtered = filtered.Where(r => r.StatusRequest == SelectedStatus);
            }

            // 4. Проверяем ДАТУ (если выбрана)
            if (SelectedDate != null)
            {
                // Сравниваем только дату (.Date), игнорируя часы и минуты
                filtered = filtered.Where(r => r.DateCreate.Date == SelectedDate.Value.Date);
            }

            // 5. Выводим результат на экран
            Requests.Clear(); // Очищаем витрину
            foreach (var item in filtered)
            {
                Requests.Add(item);
            }
        }
    }
}