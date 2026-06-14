using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TaskManager.Core;
using TaskManager.Data;
using TaskManager.Views;
using TaskManager.Service;
using System.Dynamic;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CSharp.RuntimeBinder;


public class Avtor : ObservableObject
{
private string _username; // поле логина
private string _password; // поле логина
private  AuthService _authService;
public string Username // свойство
    {
        get => _username;

        set
        {
            _username = value;
            OnPropertyChanged();
        } 
    }
 public string Password
    {
        get => _password;

        set
        {
            _password = value;
            OnPropertyChanged();

        }

    }

public ICommand LoginCommand {get;} //команда

public Avtor(AuthService authService) // Передаем сервис
{
    _authService = authService; // Сохраняем его в поле
    LoginCommand = new RelayCommand(Login); 
}

private void Login()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            MessageBox.Show(
            "Пожалуйста введите логин и пароль", 
            "Ошибка",
            MessageBoxButton.OK, 
            MessageBoxImage.Warning);
            return;
        }

       var user =_authService.Login(Username, Password); // вызвали метод

       if (user == null) // проверили на null переменную
        {
            MessageBox.Show("Неверный Логин или пароль",
            "Ошибка",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
            return;
        }
            SessionService.Instance.StartSession(user); // добавили пользователя в сессию
            
    }
}