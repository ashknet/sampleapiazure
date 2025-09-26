using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class PatientIdentifierConfiguration : IEntityTypeConfiguration<PatientIdentifier>
{
    public void Configure(EntityTypeBuilder<PatientIdentifier> builder)
    {
        builder.ToTable("PatientIdentifiers");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pi => pi.Value)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pi => pi.Issuer)
            .HasMaxLength(200);

        builder.Property(pi => pi.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(pi => new { pi.PatientId, pi.Type, pi.Value })
            .IsUnique();

        builder.HasQueryFilter(pi => !pi.IsDeleted);

        // Relationships
        builder.HasOne(pi => pi.Patient)
            .WithMany(p => p.Identifiers)
            .HasForeignKey(pi => pi.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}