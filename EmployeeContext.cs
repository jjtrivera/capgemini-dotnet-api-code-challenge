using System;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api_code_challenge;

public class EmployeeContext : DbContext
{
    public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, Name = "John Doe", Position = "Software Engineer", Salary = 80000, DateOfHire = DateTime.UtcNow.AddMonths(-15) },
            new Employee { Id = 2, Name = "Jane Smith", Position = "Project Manager", Salary = 95000, DateOfHire = DateTime.UtcNow.AddMonths(-9) }
        );
    }
}
