using System.Linq.Expressions;
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

    public DbSet<Organization> Organizations { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<UserOrganizationAssignment> UserOrganizationAssignments { get; set; } = null!;
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<PatientIdentifier> PatientIdentifiers { get; set; } = null!;
    public DbSet<PatientContact> PatientContacts { get; set; } = null!;
    public DbSet<PatientAddress> PatientAddresses { get; set; } = null!;
    public DbSet<Payer> Payers { get; set; } = null!;
    public DbSet<Plan> Plans { get; set; } = null!;
    public DbSet<Coverage> Coverages { get; set; } = null!;
    public DbSet<Consent> Consents { get; set; } = null!;
    public DbSet<ConsentEvent> ConsentEvents { get; set; } = null!;
    public DbSet<ShareToken> ShareTokens { get; set; } = null!;
    public DbSet<ConnectionLink> ConnectionLinks { get; set; } = null!;
    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<DocumentShare> DocumentShares { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<IntegrationEndpoint> IntegrationEndpoints { get; set; } = null!;
    public DbSet<NotificationPreference> NotificationPreferences { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Ignore BaseEvent as it's not an entity
        builder.Ignore<BaseEvent>();

        // Configure BaseEntity
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.IsSubclassOf(typeof(BaseEntity)))
            {
                // Ignore the DomainEvents property
                builder.Entity(entityType.ClrType)
                    .Ignore(nameof(BaseEntity.DomainEvents));

                // Add global query filter for soft delete
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var filter = Expression.Lambda(
                    Expression.Equal(property, Expression.Constant(false)),
                    parameter);
                
                builder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }

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