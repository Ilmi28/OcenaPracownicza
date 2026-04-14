using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Entities;

namespace OcenaPracownicza.API.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<ExampleEntity> ExampleEntities { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<EvaluationPeriod> EvaluationPeriods { get; set; }
    public DbSet<EvaluationCriterion> EvaluationCriteria { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

}