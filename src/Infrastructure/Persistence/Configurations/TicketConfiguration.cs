using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.PassengerId)
            .IsRequired();

        builder.Property(t => t.SeatId)
            .IsRequired();

        builder.Property(t => t.BoardingPoint)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.DroppingPoint)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(t => t.BookingReference)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.IsConfirmed)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.HasIndex(t => t.BookingReference)
            .IsUnique();

        builder.HasOne(t => t.Passenger)
            .WithMany(p => p.Tickets)
            .HasForeignKey(t => t.PassengerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Seat)
            .WithOne(s => s.Ticket)
            .HasForeignKey<Ticket>(t => t.SeatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
