using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class PatientAddressConfiguration : IEntityTypeConfiguration<PatientAddress>
{
    public void Configure(EntityTypeBuilder<PatientAddress> builder)
    {
        builder.ToTable("PatientAddresses");

        builder.HasKey(pa => pa.Id);

        builder.Property(pa => pa.Type)
            .IsRequired()
            .HasMaxLength(20);

        builder.OwnsOne(pa => pa.Address, address =>
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

        builder.Property(pa => pa.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(pa => new { pa.PatientId, pa.IsPrimary })
            .HasFilter("[IsPrimary] = 1");

        builder.HasQueryFilter(pa => !pa.IsDeleted);

        // Relationships
        builder.HasOne(pa => pa.Patient)
            .WithMany(p => p.Addresses)
            .HasForeignKey(pa => pa.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}