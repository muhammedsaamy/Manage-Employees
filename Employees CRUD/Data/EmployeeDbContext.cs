using Employees_CRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace Employees_CRUD.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<FTPFileSettings> FTPFileSettings { get; set; }

    }
}
