using HospitalService.Models.Hospitals;
using HospitalService.Models.Rooms;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HospitalService.Data;

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

    public DbSet<Hospital> Hospitals { get; set; }

    public DbSet<Room> Rooms { get; set; }
}
