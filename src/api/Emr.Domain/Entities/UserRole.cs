using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public Guid? OrganizationId { get; private set; } // Null for global roles
    public DateTime? ExpiresAt { get; private set; }
    
    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual Role Role { get; private set; } = null!;
    public virtual Organization? Organization { get; private set; }
    
    protected UserRole() { }
    
    public UserRole(Guid userId, Guid roleId, Guid? organizationId = null, DateTime? expiresAt = null)
    {
        UserId = userId;
        RoleId = roleId;
        OrganizationId = organizationId;
        ExpiresAt = expiresAt;
    }
    
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
}