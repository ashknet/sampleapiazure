using Microsoft.EntityFrameworkCore;
using Emr.Domain.Entities;

namespace Emr.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Organization> Organizations { get; }
    DbSet<Location> Locations { get; }
    DbSet<Department> Departments { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }
    DbSet<UserOrganizationAssignment> UserOrganizationAssignments { get; }
    DbSet<Patient> Patients { get; }
    DbSet<PatientIdentifier> PatientIdentifiers { get; }
    DbSet<PatientContact> PatientContacts { get; }
    DbSet<PatientAddress> PatientAddresses { get; }
    DbSet<Payer> Payers { get; }
    DbSet<Plan> Plans { get; }
    DbSet<Coverage> Coverages { get; }
    DbSet<Consent> Consents { get; }
    DbSet<ConsentEvent> ConsentEvents { get; }
    DbSet<ShareToken> ShareTokens { get; }
    DbSet<ConnectionLink> ConnectionLinks { get; }
    DbSet<Document> Documents { get; }
    DbSet<DocumentShare> DocumentShares { get; }
    DbSet<Appointment> Appointments { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<IntegrationEndpoint> IntegrationEndpoints { get; }
    DbSet<NotificationPreference> NotificationPreferences { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}