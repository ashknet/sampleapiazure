using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(al => al.Id);

        builder.Property(al => al.UserId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(al => al.UserName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(al => al.UserRole)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.Resource)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.ResourceId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(al => al.PatientId)
            .HasMaxLength(255);

        builder.Property(al => al.OrganizationId)
            .HasMaxLength(255);

        builder.Property(al => al.IpAddress)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.UserAgent)
            .HasMaxLength(500);

        builder.Property(al => al.PurposeOfUse)
            .HasMaxLength(200);

        builder.Property(al => al.ConsentId)
            .HasMaxLength(255);

        builder.Property(al => al.Success)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(al => al.FailureReason)
            .HasMaxLength(500);

        builder.Property(al => al.AdditionalData)
            .HasMaxLength(4000);

        builder.HasQueryFilter(al => !al.IsDeleted);
    }
}