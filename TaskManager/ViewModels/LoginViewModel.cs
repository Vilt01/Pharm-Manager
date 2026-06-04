using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        // свойство для логина
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
        // команда для кнопки
        public RelayCommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(TryLogin);
        }
        
        // логика входа
        private void TryLogin(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }
          
            using (var db = new AppDbContext())
            {
                var user = db.Users
                    .FirstOrDefault(u => u.Login == Username && u.Password == password);
                
                if (user != null)
                {
                    UserService.CurrentUser = user;

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is LoginView)
                        {
                            window.Close();
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка: Неверный логин или пароль.");
                }
            }
        }

    }
}
