using System.Collections.ObjectModel;
using System.Linq;
using TaskManager.Core;
using TaskManager.Models;
using TaskManager.Data; // Убедись, что AppDbContext здесь

namespace TaskManager.ViewModels
{
    public class OrdersViewModels : ObservableObject
    {
        // Список заявок, к которому вяжется таблица
        public ObservableCollection<Request> Requests { get; set; } = new ObservableCollection<Request>();

        public OrdersViewModels()
        {
            LoadData();
        }

        private void LoadData()
        {
            // Простая загрузка данных при старте
            using (var context = new AppDbContext())
            {
                var data = context.Orders.ToList();
                Requests.Clear();
                foreach (var item in data)
                {
                    Requests.Add(item);
                }
            }
        }
    }
}