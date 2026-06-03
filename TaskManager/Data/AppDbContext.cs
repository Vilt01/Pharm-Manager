using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TaskManager.Data
{
    public class AppDbContext : DbContext
    {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    var config = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json")
        .Build();
    var connectionString = config.GetConnectionString("DefaultConnection");
    optionsBuilder.UseNpgsql(connectionString);
}
        public DbSet<zapros> Requests { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
    }

}