using HospitalService.Models.Hospitals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalService.Data.Configuration;

public class HospitalEntityTypeConfiguration : IEntityTypeConfiguration<Hospital>
{
    public void Configure(EntityTypeBuilder<Hospital> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.Address);

        builder
            .HasMany(e => e.Rooms)
            .WithOne()
            .IsRequired();
    }
}
