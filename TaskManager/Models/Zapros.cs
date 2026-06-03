using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TaskManager.Models
{
    public class zapros : INotifyPropertyChanged
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Reason { get; set; }

        public string Url { get; set; }

        public string UnitMeasure { get; set; }

        public DateTime DateCreate { get; set; }

        public string StatusRequest { get; set; }

        public int? FkUser { get; set; }

        public decimal Amount { get; set; }

        public string Ozm { get; set; }

        public DateTime? DateProcess { get; set; }

        public DateTime? DateComplete { get; set; }

        private bool _isSelected;
        [NotMapped]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(); 
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
