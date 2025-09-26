using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class PayerConfiguration : IEntityTypeConfiguration<Payer>
{
    public void Configure(EntityTypeBuilder<Payer> builder)
    {
        builder.ToTable("Payers");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.PayerId)
            .HasMaxLength(50);

        builder.Property(p => p.ContactPhone)
            .HasColumnName("Phone")
            .HasMaxLength(20);

        builder.Property(p => p.ContactEmail)
            .HasColumnName("Email")
            .HasMaxLength(255);

        builder.Property(p => p.Website)
            .HasMaxLength(500);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(p => p.Code)
            .IsUnique();

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}