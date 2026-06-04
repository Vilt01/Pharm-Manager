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
    public class ZaprosItemViewModel : ItemViewModel<Zapros>
    {
        public ZaprosItemViewModel(Zapros model) : base(model) { }
    }

    public class ZaprosViewModels : ObservableObject
    {
        private readonly AppDbContext _context;

        public ObservableCollection<ZaprosItemViewModel> Requests { get; set; } = new();
        public ICollectionView RequestsView { get; private set; }

        public List<string> Statuses { get; } = new() { "Все", "Создан", "В работе", "Завершен" };

        private string _selectedStatus = "Все";
        public string SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set { _selectedDate = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilter(); }
        }

        public RelayCommand CreateCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand UpdateButtonsStateCommand { get; }

        private CreateZaprosViewModels _createRequestVM;
        public CreateZaprosViewModels CreateRequestVM
        {
            get => _createRequestVM;
            set { _createRequestVM = value; OnPropertyChanged(); }
        }

        private bool _isModalOpen;
        public bool IsModalOpen
        {
            get => _isModalOpen;
            set { _isModalOpen = value; OnPropertyChanged(); }
        }

        public ZaprosViewModels()
        {
            _context = new AppDbContext();

            CreateCommand = new RelayCommand(o => CreateRequest());
            EditCommand = new RelayCommand(o => EditRequest(), o => CanEdit());
            DeleteCommand = new RelayCommand(o => DeleteRequest(), o => CanDelete());
            UpdateButtonsStateCommand = new RelayCommand(o => CommandManager.InvalidateRequerySuggested());

            LoadData();
        }

        private void LoadData()
        {
            var data = _context.Zapros.OrderByDescending(r => r.Id).ToList();
            Requests.Clear();
            foreach (var zapros in data)
            {
                var item = new ZaprosItemViewModel(zapros);
                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(ItemViewModel<Zapros>.IsSelected))
                        UpdateButtonsStateCommand.Execute(null);
                };
                Requests.Add(item);
            }

            // Создаём ICollectionView один раз
            RequestsView = CollectionViewSource.GetDefaultView(Requests);
            RequestsView.Filter = o => FilterPredicate(o as ZaprosItemViewModel);
            ApplyFilter(); // применяем текущий фильтр
        }

        private bool FilterPredicate(ZaprosItemViewModel wrapper)
        {
            if (wrapper == null) return false;
            var model = wrapper.Model;

            bool matchesSearch = string.IsNullOrWhiteSpace(SearchText) ||
                (model.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false);

            bool matchesStatus = SelectedStatus == "Все" || model.StatusRequest == SelectedStatus;

            bool matchesDate = !SelectedDate.HasValue || model.DateCreate == DateOnly.FromDateTime(SelectedDate.Value);

            return matchesSearch && matchesStatus && matchesDate;
        }

        private void ApplyFilter()
        {
            RequestsView?.Refresh(); // просто обновляем фильтр
        }

        private bool CanEdit()
        {
            var selected = Requests.Where(r => r.IsSelected).ToList();
            return selected.Count == 1 && selected[0].Model.StatusRequest == "Создан";
        }

        private bool CanDelete()
        {
            var selected = Requests.Where(r => r.IsSelected).ToList();
            if (selected.Count == 0) return false;
            return selected.All(r => r.Model.StatusRequest == "Создан");
        }

        private void CreateRequest()
        {
            var vm = new CreateZaprosViewModels();
            vm.RequestClose += () =>
            {
                IsModalOpen = false;
                CreateRequestVM = null;
                LoadData(); // перезагружаем данные после создания
            };
            CreateRequestVM = vm;
            IsModalOpen = true;
        }

        private void EditRequest()
        {
            var item = Requests.FirstOrDefault(r => r.IsSelected);
            if (item == null) return;
            MessageBox.Show($"Редактировать ID: {item.Model.Id}");
        }

        private void DeleteRequest()
        {
            var itemsToDelete = Requests.Where(r => r.IsSelected).ToList();
            if (itemsToDelete.Count == 0) return;

            if (MessageBox.Show($"Вы уверены, что хотите удалить {itemsToDelete.Count} заявок?",
                                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    var modelsToDelete = itemsToDelete.Select(w => w.Model).ToList();
                    _context.Zapros.RemoveRange(modelsToDelete);
                    _context.SaveChanges();
                    LoadData(); // перезагружаем данные после удаления
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    MessageBox.Show("Невозможно удалить одну или несколько заявок.\n\nПричина: Эти заявки используются в других таблицах (например, в Журнале действий).",
                                    "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                    _context.ChangeTracker.Clear();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}