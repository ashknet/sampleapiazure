using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class DocumentShareConfiguration : IEntityTypeConfiguration<DocumentShare>
{
    public void Configure(EntityTypeBuilder<DocumentShare> builder)
    {
        builder.ToTable("DocumentShares");

        builder.HasKey(ds => ds.Id);

        builder.Property(ds => ds.Purpose)
            .HasMaxLength(500);

        builder.Property(ds => ds.CanDownload)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ds => ds.CanPrint)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ds => ds.AccessCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasQueryFilter(ds => !ds.IsDeleted);

        // Relationships
        builder.HasOne(ds => ds.Document)
            .WithMany(d => d.DocumentShares)
            .HasForeignKey(ds => ds.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ds => ds.SharedWithUser)
            .WithMany()
            .HasForeignKey(ds => ds.SharedWithUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ds => ds.SharedByUser)
            .WithMany()
            .HasForeignKey(ds => ds.SharedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}