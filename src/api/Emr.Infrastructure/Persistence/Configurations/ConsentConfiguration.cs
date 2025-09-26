using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class ConsentConfiguration : IEntityTypeConfiguration<Consent>
{
    public void Configure(EntityTypeBuilder<Consent> builder)
    {
        builder.ToTable("Consents");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.PurposeOfUse)
            .HasMaxLength(200);

        builder.Property(c => c.ConsentingParty)
            .HasMaxLength(255);

        builder.Property(c => c.RevokedBy)
            .HasMaxLength(255);

        builder.Property(c => c.ScopeJson)
            .IsRequired()
            .HasMaxLength(4000);

        builder.HasQueryFilter(c => !c.IsDeleted);

        // Relationships
        builder.HasOne(c => c.Patient)
            .WithMany(p => p.Consents)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Organization)
            .WithMany()
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}