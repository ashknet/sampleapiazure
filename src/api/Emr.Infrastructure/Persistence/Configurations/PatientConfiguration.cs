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
            .HasMaxLength(11);

        builder.Property(p => p.Gender)
            .IsRequired()
            .HasMaxLength(20);

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

        builder.HasIndex(p => p.MedicalRecordNumber)
            .IsUnique();

        builder.HasIndex(p => p.SocialSecurityNumber)
            .IsUnique()
            .HasFilter("[SocialSecurityNumber] IS NOT NULL");

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}