using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ToTable("Routes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.FromCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.ToCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Distance)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(r => r.EstimatedDuration)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasIndex(r => new { r.FromCity, r.ToCity });

        builder.HasMany(r => r.BusSchedules)
            .WithOne(bs => bs.Route)
            .HasForeignKey(bs => bs.RouteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
