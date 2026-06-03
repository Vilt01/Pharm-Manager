using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TaskManager.Core;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.ViewModels
{
    public class CreateUserViewModel : ObservableObject
    {
        private User _editingUser;

        private string _windowTitle = "Добавление пользователя";
        public string WindowTitle
        {
            get => _windowTitle;
            set { _windowTitle = value; OnPropertyChanged(); }
        }

        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private string _surName;
        public string SurName { get => _surName; set { _surName = value; OnPropertyChanged(); } }

        private string _lastName;
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }

        private string _numberPhone;
        public string NumberPhone { get => _numberPhone; set { _numberPhone = value; OnPropertyChanged(); } }

        private string _mail;
        public string Mail { get => _mail; set { _mail = value; OnPropertyChanged(); } }

        private string _login;
        public string Login { get => _login; set { _login = value; OnPropertyChanged(); } }

        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        private Role _selectedRole;
        public Role SelectedRole { get => _selectedRole; set { _selectedRole = value; OnPropertyChanged(); } }

        private Department _selectedDepartment;
        public Department SelectedDepartment { get => _selectedDepartment; set { _selectedDepartment = value; OnPropertyChanged(); } }

        public ObservableCollection<Role> Roles { get; set; }
        public ObservableCollection<Department> Departments { get; set; }

        public Action RequestClose { get; set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public CreateUserViewModel(User userToEdit = null)
        {
            _editingUser = userToEdit;

            using (var db = new AppDbContext())
            {
                Roles = new ObservableCollection<Role>(db.Roles.ToList());
                Departments = new ObservableCollection<Department>(db.Departments.ToList());
            }

            if (_editingUser != null)
            {
                WindowTitle = "Редактирование пользователя";
                Name = _editingUser.name;
                SurName = _editingUser.surname;
                LastName = _editingUser.lastname;
                NumberPhone = _editingUser.phone;
                Mail = _editingUser.mail;
                Login = _editingUser.login;
                Password = _editingUser.password;

                SelectedRole = Roles.FirstOrDefault(r => r.Id == _editingUser.fk_role);
                SelectedDepartment = Departments.FirstOrDefault(d => d.Id == _editingUser.fk_department);
            }

            SaveCommand = new RelayCommand(o => SaveData());
            CancelCommand = new RelayCommand(o => RequestClose?.Invoke());
        }

        private void SaveData()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(SurName) ||
                string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password) || SelectedRole == null)
            {
                MessageBox.Show("Заполните все обязательные поля!");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    if (_editingUser == null)
                    {
                        var newUser = new User
                        {
                            name = Name,
                            surname = SurName,
                            lastname = LastName,
                            phone = NumberPhone,
                            mail = Mail,
                            login = Login,
                            password = Password,
                            fk_role = SelectedRole.Id,
                            fk_department = SelectedDepartment?.Id
                        };
                        db.User.Add(newUser);
                    }
                    else
                    {
                        var userToUpdate = db.User.FirstOrDefault(u => u.id == _editingUser.id);
                        if (userToUpdate != null)
                        {
                            userToUpdate.name = Name;
                            userToUpdate.surname = SurName;
                            userToUpdate.lastname = LastName;
                            userToUpdate.phone = NumberPhone;
                            userToUpdate.mail = Mail;
                            userToUpdate.login = Login;
                            userToUpdate.password = Password;
                            userToUpdate.fk_role = SelectedRole.Id;
                            userToUpdate.fk_department = SelectedDepartment?.Id;
                        }
                    }

                    db.SaveChanges();
                }
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении в БД: " + ex.Message);
            }
        }
    }
}