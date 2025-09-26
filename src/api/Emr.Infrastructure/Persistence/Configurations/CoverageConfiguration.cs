using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class CoverageConfiguration : IEntityTypeConfiguration<Coverage>
{
    public void Configure(EntityTypeBuilder<Coverage> builder)
    {
        builder.ToTable("Coverages");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.MemberId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.GroupNumber)
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(c => new { c.PatientId, c.Priority });

        builder.HasQueryFilter(c => !c.IsDeleted);

        // Relationships
        builder.HasOne(c => c.Patient)
            .WithMany(p => p.Coverages)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Plan)
            .WithMany()
            .HasForeignKey(c => c.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}