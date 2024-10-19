using DocumentService.Models.History;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DocumentService.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<History> Histories { get; set; }
}
