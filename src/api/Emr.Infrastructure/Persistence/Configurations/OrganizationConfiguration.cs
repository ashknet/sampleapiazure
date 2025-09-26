using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.TaxId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.NpiNumber)
            .HasMaxLength(10);

        builder.Property(o => o.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.ContactEmail)
            .HasMaxLength(255);

        builder.Property(o => o.ContactPhone)
            .HasMaxLength(20);

        builder.Property(o => o.Website)
            .HasMaxLength(255);

        builder.Property(o => o.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(o => o.TaxId)
            .IsUnique();

        builder.HasIndex(o => o.NpiNumber)
            .IsUnique()
            .HasFilter("[NpiNumber] IS NOT NULL");

        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}