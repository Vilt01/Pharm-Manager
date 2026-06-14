using System.Net.Http.Headers;
using Models;
public class AuthService 
{
    public User? Login(string username, string password)
    {   
        using var db = new AppDbContext(); //подключение к бд

        var user = db.User.FirstOrDefault
        (u => u.Login ==username); // проверка логина

        if (user == null) //пользователь в бд не найден
        return null;

        if (user.IsDeleted == true) //статус пользователя
        return null;

        if (user.Password != password) //пароли не равны
        return null;

        return user;
    }
}