using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string sur_name { get; set; }
        public string last_name { get; set; }
        public string number_phone { get; set; }
        public string mail { get; set; } 
        public string login { get; set; }
        public string password { get; set; }
        public int? fk_role { get; set; } 

    }
}
