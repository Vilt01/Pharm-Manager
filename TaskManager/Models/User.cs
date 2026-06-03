using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace TaskManager.Models
{
    public class User : INotifyPropertyChanged
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string lastname { get; set; }
        public string phone { get; set; }
        public string mail { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public byte[]? avatar { get; set; }
        public int? fk_role { get; set; }
        public int? fk_manager_id { get; set; }
        public int? fk_department { get; set; }
        public bool is_deleted { get; set; }

        private bool _isSelected;
        [NotMapped]
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        [NotMapped]
        public string FullName => $"{surname} {name} {lastname}".Trim();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}