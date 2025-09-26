using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class ConnectionLinkConfiguration : IEntityTypeConfiguration<ConnectionLink>
{
    public void Configure(EntityTypeBuilder<ConnectionLink> builder)
    {
        builder.ToTable("ConnectionLinks");

        builder.HasKey(cl => cl.Id);

        builder.Property(cl => cl.AccessToken)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(cl => cl.IpAddress)
            .HasMaxLength(50);

        builder.Property(cl => cl.UserAgent)
            .HasMaxLength(500);

        builder.Property(cl => cl.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasQueryFilter(cl => !cl.IsDeleted);

        // Relationships
        builder.HasOne(cl => cl.ShareToken)
            .WithMany(st => st.ConnectionLinks)
            .HasForeignKey(cl => cl.ShareTokenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}