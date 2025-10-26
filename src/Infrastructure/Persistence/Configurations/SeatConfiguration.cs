using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("Seats");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.BusScheduleId)
            .IsRequired();

        builder.Property(s => s.SeatNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.Row)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasIndex(s => new { s.BusScheduleId, s.SeatNumber })
            .IsUnique();

        builder.HasOne(s => s.BusSchedule)
            .WithMany(bs => bs.Seats)
            .HasForeignKey(s => s.BusScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Ticket)
            .WithOne(t => t.Seat)
            .HasForeignKey<Ticket>(t => t.SeatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
