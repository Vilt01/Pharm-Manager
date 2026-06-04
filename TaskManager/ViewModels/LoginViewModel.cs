using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Core;
using TaskManager.Data;
using TaskManager.Views;
using TaskManager.Service;

namespace TaskManager.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private bool _showPassword;
        public bool ShowPassword
        {
            get => _showPassword;
            set
            {
                _showPassword = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand LoginCommand { get; set; }
        public RelayCommand TogglePasswordCommand { get; set; }
        public RelayCommand ForgotPasswordCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(TryLogin);
            TogglePasswordCommand = new RelayCommand(_ => ShowPassword = !ShowPassword);
            ForgotPasswordCommand = new RelayCommand(_ => OpenForgotPasswordDialog());
        }

        private void TryLogin(object parameter)
        {
            string password = Password;
            if (parameter is PasswordBox pb)
                password = pb.Password;

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var db = new AppDbContext();
                var user = db.Users.FirstOrDefault(u => u.Login == Username && u.Password == password);

                if (user != null)
                {
                    UserService.CurrentUser = user;

                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    foreach (Window window in Application.Current.Windows)
                        if (window is LoginView) window.Close();
                }
                else
                {
                    MessageBox.Show("Ошибка: Неверный логин или пароль.", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenForgotPasswordDialog()
        {
            MessageBox.Show("Если вы забыли пароль, обратитесь в отдел IT за необходимой помощью.", "Восстановление пароля", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}