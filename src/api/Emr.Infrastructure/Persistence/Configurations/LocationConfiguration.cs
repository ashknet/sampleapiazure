using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.Phone)
            .HasMaxLength(20);

        builder.Property(l => l.Fax)
            .HasMaxLength(20);

        builder.Property(l => l.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.OwnsOne(l => l.Address, address =>
        {
            address.Property(a => a.Street1)
                .HasColumnName("Street1")
                .IsRequired()
                .HasMaxLength(200);

            address.Property(a => a.Street2)
                .HasColumnName("Street2")
                .HasMaxLength(200);

            address.Property(a => a.City)
                .HasColumnName("City")
                .IsRequired()
                .HasMaxLength(100);

            address.Property(a => a.State)
                .HasColumnName("State")
                .IsRequired()
                .HasMaxLength(2);

            address.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .IsRequired()
                .HasMaxLength(10);

            address.Property(a => a.Country)
                .HasColumnName("Country")
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.HasIndex(l => new { l.OrganizationId, l.Code })
            .IsUnique();

        builder.HasQueryFilter(l => !l.IsDeleted);

        // Relationships
        builder.HasOne(l => l.Organization)
            .WithMany(o => o.Locations)
            .HasForeignKey(l => l.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}