using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.MedicalRecordNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.SocialSecurityNumber)
            .HasMaxLength(20);

        builder.Property(p => p.Gender)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.PreferredLanguage)
            .HasMaxLength(10);

        builder.Property(p => p.Race)
            .HasMaxLength(50);

        builder.Property(p => p.Ethnicity)
            .HasMaxLength(50);

        builder.Property(p => p.MaritalStatus)
            .HasMaxLength(20);

        builder.Property(p => p.Religion)
            .HasMaxLength(50);

        builder.Property(p => p.IsDeceased)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(p => p.MedicalRecordNumber)
            .IsUnique();

        builder.HasIndex(p => p.UserId);

        builder.HasQueryFilter(p => !p.IsDeleted);

        // Relationships
        builder.HasOne(p => p.User)
            .WithOne(u => u.PatientProfile)
            .HasForeignKey<Patient>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}