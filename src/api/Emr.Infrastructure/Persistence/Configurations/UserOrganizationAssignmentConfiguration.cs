using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Persistence.Configurations;

public class UserOrganizationAssignmentConfiguration : IEntityTypeConfiguration<UserOrganizationAssignment>
{
    public void Configure(EntityTypeBuilder<UserOrganizationAssignment> builder)
    {
        builder.ToTable("UserOrganizationAssignments");

        builder.HasKey(ua => ua.Id);

        builder.Property(ua => ua.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasQueryFilter(ua => !ua.IsDeleted);

        // Relationships
        builder.HasOne(ua => ua.User)
            .WithMany(u => u.OrganizationAssignments)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ua => ua.Organization)
            .WithMany(o => o.UserAssignments)
            .HasForeignKey(ua => ua.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ua => ua.Department)
            .WithMany()
            .HasForeignKey(ua => ua.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}