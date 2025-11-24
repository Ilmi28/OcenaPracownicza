using Microsoft.EntityFrameworkCore;
using OcenaPracownicza.API.Data;

public class TestDbContext : ApplicationDbContext
{
    public TestDbContext(DbContextOptions options)
        : base(options)
    {
    }
}
