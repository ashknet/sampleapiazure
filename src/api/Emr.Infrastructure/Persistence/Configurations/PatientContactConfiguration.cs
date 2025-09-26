using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class PatientContactConfiguration : IEntityTypeConfiguration<PatientContact>
{
    public void Configure(EntityTypeBuilder<PatientContact> builder)
    {
        builder.ToTable("PatientContacts");

        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pc => pc.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pc => pc.Relationship)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pc => pc.Phone)
            .HasMaxLength(20);

        builder.Property(pc => pc.Email)
            .HasMaxLength(255);

        builder.Property(pc => pc.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasQueryFilter(pc => !pc.IsDeleted);

        // Relationships
        builder.HasOne(pc => pc.Patient)
            .WithMany(p => p.Contacts)
            .HasForeignKey(pc => pc.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}