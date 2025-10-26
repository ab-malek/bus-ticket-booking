using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BusScheduleConfiguration : IEntityTypeConfiguration<BusSchedule>
{
    public void Configure(EntityTypeBuilder<BusSchedule> builder)
    {
        builder.ToTable("BusSchedules");

        builder.HasKey(bs => bs.Id);

        builder.Property(bs => bs.BusId)
            .IsRequired();

        builder.Property(bs => bs.RouteId)
            .IsRequired();

        builder.Property(bs => bs.JourneyDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(bs => bs.DepartureTime)
            .IsRequired();

        builder.Property(bs => bs.ArrivalTime)
            .IsRequired();

        builder.Property(bs => bs.Fare)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(bs => bs.BoardingPoint)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(bs => bs.DroppingPoint)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(bs => bs.CreatedAt)
            .IsRequired();

        builder.HasIndex(bs => new { bs.BusId, bs.RouteId, bs.JourneyDate });

        builder.HasOne(bs => bs.Bus)
            .WithMany(b => b.BusSchedules)
            .HasForeignKey(bs => bs.BusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bs => bs.Route)
            .WithMany(r => r.BusSchedules)
            .HasForeignKey(bs => bs.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(bs => bs.Seats)
            .WithOne(s => s.BusSchedule)
            .HasForeignKey(s => s.BusScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
