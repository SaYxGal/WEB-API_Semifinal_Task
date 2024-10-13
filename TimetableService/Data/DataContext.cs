using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TimetableService.Models.Timetables;

namespace TimetableService.Data;

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

    public DbSet<Timetable> Timetables { get; set; }
}
