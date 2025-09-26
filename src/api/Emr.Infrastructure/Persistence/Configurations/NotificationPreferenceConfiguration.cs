using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("NotificationPreferences");

        builder.HasKey(np => np.Id);

        builder.Property(np => np.Channel)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(np => np.NotificationType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(np => np.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(np => np.Frequency)
            .HasMaxLength(50);

        builder.Property(np => np.AdditionalSettings)
            .HasMaxLength(4000);

        builder.HasIndex(np => new { np.UserId, np.Channel, np.NotificationType })
            .IsUnique();

        builder.HasQueryFilter(np => !np.IsDeleted);

        // Relationships
        builder.HasOne(np => np.User)
            .WithMany()
            .HasForeignKey(np => np.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}