using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core;
using TaskManager.Data;
using TaskManager.Models;
using System.Collections.Generic;

namespace TaskManager.ViewModels
{
    public class UsersViewModel : ObservableObject
    {
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        private ICollectionView _usersView;
        public ICollectionView UsersView
        {
            get => _usersView;
            set { _usersView = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Role> Roles { get; set; } = new ObservableCollection<Role>();

        private string _searchText;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); ApplyFilter(); } }

        private Role _selectedRole;
        public Role SelectedRole { get => _selectedRole; set { _selectedRole = value; OnPropertyChanged(); ApplyFilter(); } }

        private CreateUserViewModel _createUserVM;
        public CreateUserViewModel CreateUserVM { get => _createUserVM; set { _createUserVM = value; OnPropertyChanged(); } }

        private bool _isModalOpen;
        public bool IsModalOpen { get => _isModalOpen; set { _isModalOpen = value; OnPropertyChanged(); } }

        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand ClearFilterCommand { get; }

        public UsersViewModel()
        {
            AddCommand = new RelayCommand(o => OpenCreateUserWindow());
            EditCommand = new RelayCommand(o => OpenEditUserWindow(), o => Users.Any(u => u.IsSelected));
            DeleteCommand = new RelayCommand(o => DeleteSelectedUsers(), o => Users.Any(u => u.IsSelected));
            ClearFilterCommand = new RelayCommand(o => SelectedRole = null);

            LoadInitialData();
        }

        private void LoadInitialData()
        {
            using (var db = new AppDbContext())
            {
                var roles = db.Roles.ToList().GroupBy(r => r.Name).Select(g => g.First()).ToList();
                Roles.Clear();
                foreach (var r in roles) Roles.Add(r);
            }
            RefreshUsers();
        }

        public void RefreshUsers()
        {
            Users.Clear();
            using (var db = new AppDbContext())
            {
                // ИНТЕГРАЦИЯ ШАГА 3: Загружаем только тех, у кого IsDeleted == false
                var data = db.User
                    .Include(u => u.fk_role)
                    .Include(u => u.fk_department)
                    .Where(u => !u.is_deleted)
                    .ToList();

                foreach (var user in data)
                {
                    user.IsSelected = false; // Сбрасываем выбор
                    user.PropertyChanged += (s, e) => {
                        if (e.PropertyName == nameof(User.IsSelected))
                            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    };
                    Users.Add(user);
                }
            }
            UsersView = CollectionViewSource.GetDefaultView(Users);
            UsersView.Refresh();
        }

        private void ApplyFilter()
        {
            if (UsersView == null) return;

            UsersView.Filter = o =>
            {
                var user = o as User;
                if (user == null) return false;

                // Если строка поиска пустая — показываем всех
                if (string.IsNullOrWhiteSpace(SearchText)) return SelectedRole == null || user.fk_role == SelectedRole.Id;

                // Проверка по всем полям (регистронезависимая)
                bool matchesSearch =
                    (user.name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (user.surname?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (user.lastname?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (user.login?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (user.mail?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (user.phone?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false);

                // Учитываем также выбранную роль в левом меню
                bool matchesRole = SelectedRole == null || user.fk_role == SelectedRole.Id;

                return matchesSearch && matchesRole;
            };
        }

        private void OpenCreateUserWindow()
        {
            var vm = new CreateUserViewModel();
            vm.RequestClose += () => { IsModalOpen = false; CreateUserVM = null; RefreshUsers(); };
            CreateUserVM = vm;
            IsModalOpen = true;
        }

        private void OpenEditUserWindow()
        {
            var selected = Users.FirstOrDefault(u => u.IsSelected);
            if (selected == null) return;

            var vm = new CreateUserViewModel(selected);
            vm.RequestClose += () => { IsModalOpen = false; CreateUserVM = null; RefreshUsers(); };
            CreateUserVM = vm;
            IsModalOpen = true;
        }

        private void DeleteSelectedUsers()
        {
            var toDelete = Users.Where(u => u.IsSelected).ToList();
            if (toDelete.Count == 0) return;

            if (MessageBox.Show($"Скрыть выбранных пользователей ({toDelete.Count} шт.)?",
                                "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new AppDbContext())
                {
                    // ИНТЕГРАЦИЯ ШАГА 3: Вместо физического удаления меняем статус IsDeleted
                    var ids = toDelete.Select(u => u.id).ToList();
                    var usersInDb = db.User.Where(u => ids.Contains(u.id)).ToList();

                    foreach (var user in usersInDb)
                    {
                        user.is_deleted = true;
                    }

                    db.SaveChanges();
                }
                RefreshUsers(); // Обновляем список, чтобы помеченные записи исчезли
            }
        }
    }
}