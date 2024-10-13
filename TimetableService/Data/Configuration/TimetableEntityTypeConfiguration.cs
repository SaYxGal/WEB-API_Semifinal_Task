using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimetableService.Models.Timetables;

namespace TimetableService.Data.Configuration;

public class TimetableEntityTypeConfiguration : IEntityTypeConfiguration<Timetable>
{
    public void Configure(EntityTypeBuilder<Timetable> builder)
    {
        builder.HasKey(i => i.Id);

        builder
            .HasMany(e => e.Appointments)
            .WithOne()
            .HasForeignKey(e => e.TimetableId)
            .IsRequired();
    }
}
