using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data.Identity;
using OcenaPracownicza.API.Entities;

namespace OcenaPracownicza.API.Data;

public class ApplicationDbContext : IdentityDbContext<BaseUser>
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<ExampleEntity> ExampleEntities { get; set; }
    public DbSet<Employee> Employees { get; set; }
}