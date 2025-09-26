using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class ConsentEventConfiguration : IEntityTypeConfiguration<ConsentEvent>
{
    public void Configure(EntityTypeBuilder<ConsentEvent> builder)
    {
        builder.ToTable("ConsentEvents");

        builder.HasKey(ce => ce.Id);

        builder.Property(ce => ce.EventType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ce => ce.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ce => ce.PerformedBy)
            .HasMaxLength(255);

        builder.HasQueryFilter(ce => !ce.IsDeleted);

        // Relationships
        builder.HasOne(ce => ce.Consent)
            .WithMany(c => c.Events)
            .HasForeignKey(ce => ce.ConsentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}