using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimetableService.Models.Appointments;
using TimetableService.Models.Timetables;

namespace TimetableService.Data.Configuration;

public class AppointmentEntityTypeConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(i => new { i.TimetableId, i.Time });

        builder
            .HasOne<Timetable>()
            .WithMany(e => e.Appointments)
            .HasForeignKey(e => e.TimetableId)
            .IsRequired();
    }
}
