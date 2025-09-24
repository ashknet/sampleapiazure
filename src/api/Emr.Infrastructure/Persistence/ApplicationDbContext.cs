using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Emr.Application.Common.Interfaces;
using Emr.Domain.Common;
using Emr.Domain.Entities;
using Emr.Infrastructure.Persistence.Interceptors;

namespace Emr.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDateTime dateTime,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) 
        : base(options)
    {
        _currentUserService = currentUserService;
        _dateTime = dateTime;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserOrganizationAssignment> UserOrganizationAssignments => Set<UserOrganizationAssignment>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientIdentifier> PatientIdentifiers => Set<PatientIdentifier>();
    public DbSet<PatientContact> PatientContacts => Set<PatientContact>();
    public DbSet<PatientAddress> PatientAddresses => Set<PatientAddress>();
    public DbSet<Payer> Payers => Set<Payer>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Coverage> Coverages => Set<Coverage>();
    public DbSet<Consent> Consents => Set<Consent>();
    public DbSet<ConsentEvent> ConsentEvents => Set<ConsentEvent>();
    public DbSet<ShareToken> ShareTokens => Set<ShareToken>();
    public DbSet<ConnectionLink> ConnectionLinks => Set<ConnectionLink>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentShare> DocumentShares => Set<DocumentShare>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<IntegrationEndpoint> IntegrationEndpoints => Set<IntegrationEndpoint>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedInfo(_currentUserService.UserId ?? "system");
                    break;

                case EntityState.Modified:
                    entry.Entity.SetUpdatedInfo(_currentUserService.UserId ?? "system");
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}