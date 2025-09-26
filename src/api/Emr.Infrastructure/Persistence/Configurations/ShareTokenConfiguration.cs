using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class ShareTokenConfiguration : IEntityTypeConfiguration<ShareToken>
{
    public void Configure(EntityTypeBuilder<ShareToken> builder)
    {
        builder.ToTable("ShareTokens");

        builder.HasKey(st => st.Id);

        builder.Property(st => st.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(st => st.Type)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(st => st.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(st => st.RequestedScopes)
            .HasMaxLength(4000);

        builder.Property(st => st.AuthorizedScopes)
            .HasMaxLength(4000);

        builder.Property(st => st.AuthorizedBy)
            .HasMaxLength(255);

        builder.Property(st => st.AccessCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(st => st.Code)
            .IsUnique();

        builder.HasQueryFilter(st => !st.IsDeleted);

        // Relationships
        builder.HasOne(st => st.Patient)
            .WithMany(p => p.ShareTokens)
            .HasForeignKey(st => st.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(st => st.Organization)
            .WithMany()
            .HasForeignKey(st => st.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}