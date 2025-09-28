using Microsoft.EntityFrameworkCore;
using AzureSecureAPI.Models;

namespace AzureSecureAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed some initial data
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "John Doe", Email = "john.doe@example.com", Department = "IT", Salary = 75000 },
                new Employee { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", Department = "HR", Salary = 65000 },
                new Employee { Id = 3, Name = "Bob Johnson", Email = "bob.johnson@example.com", Department = "Sales", Salary = 70000 }
            );
        }
    }
}