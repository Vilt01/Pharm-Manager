using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TaskManager.Models
{
    [Table("Request")]
    public class Request
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string reason { get; set; }
        public string url { get; set; }
        public string unit_measure { get; set; }
        public int number_order { get; set; }
        public DateTime date_create { get; set; }
        public string status_requestion { get; set; }
        public int? fk_user { get; set; }
        public int amount { get; set; }
    }
}
