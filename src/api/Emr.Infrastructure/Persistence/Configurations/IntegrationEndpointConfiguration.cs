using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class IntegrationEndpointConfiguration : IEntityTypeConfiguration<IntegrationEndpoint>
{
    public void Configure(EntityTypeBuilder<IntegrationEndpoint> builder)
    {
        builder.ToTable("IntegrationEndpoints");

        builder.HasKey(ie => ie.Id);

        builder.Property(ie => ie.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ie => ie.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ie => ie.Direction)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ie => ie.EndpointUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ie => ie.AuthType)
            .HasMaxLength(50);

        builder.Property(ie => ie.ConfigurationJson)
            .HasMaxLength(4000);

        builder.Property(ie => ie.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(ie => ie.LastError)
            .HasMaxLength(500);

        builder.HasQueryFilter(ie => !ie.IsDeleted);

        // Relationships
        builder.HasOne(ie => ie.Organization)
            .WithMany()
            .HasForeignKey(ie => ie.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}