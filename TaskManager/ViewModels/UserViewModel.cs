using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.ViewModels
{
    public class UsersViewModel : ObservableObject
    {
        public ObservableCollection<UserItemViewModel> Users { get; set; } = new ObservableCollection<UserItemViewModel>();

        private ICollectionView _usersView;
        public ICollectionView UsersView
        {
            get => _usersView;
            set { _usersView = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Role> Roles { get; set; } = new ObservableCollection<Role>();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private Role _selectedRole;
        public Role SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); ApplyFilter(); }
        }

        private CreateUserViewModel _createUserVM;
        public CreateUserViewModel CreateUserVM
        {
            get => _createUserVM;
            set { _createUserVM = value; OnPropertyChanged(); }
        }

        private bool _isModalOpen;
        public bool IsModalOpen
        {
            get => _isModalOpen;
            set { _isModalOpen = value; OnPropertyChanged(); }
        }

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
            using (var db = new AppDbContext())
            {
                var usersFromDb = db.Users
                    .Include(u => u.FkRoleNavigation)
                    .Include(u => u.FkDepartmentNavigation)
                    .Where(u => u.IsDeleted == false)
                    .ToList();

                Users.Clear();
                foreach (var user in usersFromDb)
                {
                    Users.Add(new UserItemViewModel(user));
                }
            }
            UsersView = CollectionViewSource.GetDefaultView(Users);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (UsersView == null) return;

            UsersView.Filter = o =>
            {
                var wrapper = o as UserItemViewModel;
                if (wrapper == null) return false;

                var user = wrapper.Model;

                bool matchesSearch = string.IsNullOrWhiteSpace(SearchText) ||
                    (user.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (user.Surname?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (user.Lastname?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (user.Login?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (user.Mail?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (user.Phone?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false);

                bool matchesRole = SelectedRole == null || user.FkRole == SelectedRole.Id;

                return matchesSearch && matchesRole;
            };
            UsersView.Refresh();
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
            var selectedWrapper = Users.FirstOrDefault(u => u.IsSelected);
            if (selectedWrapper == null) return;

            var vm = new CreateUserViewModel(selectedWrapper.Model);
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
                    var ids = toDelete.Select(w => w.Model.Id).ToList();
                    var usersInDb = db.Users.Where(u => ids.Contains(u.Id)).ToList();

                    foreach (var user in usersInDb)
                    {
                        user.IsDeleted = true;
                    }
                    db.SaveChanges();
                }
                RefreshUsers();
            }
        }
    }
}