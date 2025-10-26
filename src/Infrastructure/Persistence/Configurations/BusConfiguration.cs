using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BusConfiguration : IEntityTypeConfiguration<Bus>
{
    public void Configure(EntityTypeBuilder<Bus> builder)
    {
        builder.ToTable("Buses");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.BusName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.BusNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(b => b.BusNumber)
            .IsUnique();

        builder.Property(b => b.BusType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.TotalSeats)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.HasMany(b => b.BusSchedules)
            .WithOne(bs => bs.Bus)
            .HasForeignKey(bs => bs.BusId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
