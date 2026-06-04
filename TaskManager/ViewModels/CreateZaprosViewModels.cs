using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Core;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Service;

namespace TaskManager.ViewModels
{
   public class CreateZaprosViewModels : ObservableObject
    {
        public string Name { get; set; }
        public string Ozm { get; set; }
        public string Reason { get; set; }
        public string StringAmount { get; set; }
        public string UnitMeasure { get; set; }
        public string Url { get; set; }
        public Action RequestClose { get; set; }
        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; set; }
       
        public CreateZaprosViewModels() 
        {
            SaveCommand = new RelayCommand(o => SaveData());

            CancelCommand = new RelayCommand(o => RequestClose?.Invoke());

        }
        
        private void SaveData()
        {
            if (UserService.CurrentUser == null)
            {
                MessageBox.Show("Ошибка: Вы не авторизованы! перезапустите программу и выполните вход");
                return;
            }

            //Конвертируем в число строку количество
            if (!decimal.TryParse(StringAmount, out decimal finalAmount))
            {
                MessageBox.Show("Ошибка, необходимо ввести число");
                return;
            }
            
            var newRequest = new Zapros
            {
                Name = Name,
                Ozm = Ozm,
                Reason = Reason,
                Amount = finalAmount,
                UnitMeasure = UnitMeasure,
                Url = Url??"",
                DateCreate = DateOnly.FromDateTime(DateTime.UtcNow),
                StatusRequest = "Создан",
                FkUser = UserService.CurrentUser.Id
            };

            try
            {
                using (var db = new AppDbContext())
                {
                    db.Zapros.Add(newRequest);
                    db.SaveChanges();
                }

                RequestClose?.Invoke();
            }
            
            catch(Exception ex)
            {
                string error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show("Ошибка БД " + error);
            }
        }
        


    }
}
