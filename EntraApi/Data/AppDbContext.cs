using EntraApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EntraApi.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  public DbSet<Employee> Employees => Set<Employee>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Employee>(entity =>
    {
      entity.ToTable("employee");
      entity.HasKey(e => e.Id);
      entity.Property(e => e.Id).HasColumnName("id");
      entity.Property(e => e.Name).HasColumnName("name");
      entity.Property(e => e.Title).HasColumnName("title");
      entity.Property(e => e.Email).HasColumnName("email");
    });

    base.OnModelCreating(modelBuilder);
  }
}

