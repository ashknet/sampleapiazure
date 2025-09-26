using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.DocumentType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Category)
            .HasMaxLength(100);

        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.BlobStorageUrl)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(d => d.Checksum)
            .HasMaxLength(100);

        builder.Property(d => d.IsConfidential)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(d => d.ConfidentialityCode)
            .HasMaxLength(50);

        builder.Property(d => d.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.FhirDocumentReferenceId)
            .HasMaxLength(500);

        builder.HasQueryFilter(d => !d.IsDeleted);

        // Relationships
        builder.HasOne(d => d.Patient)
            .WithMany(p => p.Documents)
            .HasForeignKey(d => d.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.UploadedByUser)
            .WithMany()
            .HasForeignKey(d => d.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Organization)
            .WithMany()
            .HasForeignKey(d => d.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}