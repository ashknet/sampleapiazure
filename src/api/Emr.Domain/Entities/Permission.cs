using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class Permission : BaseEntity
{
    public string Resource { get; private set; } = string.Empty; // e.g., "Patient", "Document", "Appointment"
    public string Action { get; private set; } = string.Empty; // e.g., "Read", "Write", "Delete"
    public string Scope { get; private set; } = string.Empty; // e.g., "Own", "Organization", "All"
    public string? Description { get; private set; }
    
    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    
    protected Permission() { }
    
    public Permission(string resource, string action, string scope)
    {
        Resource = resource;
        Action = action;
        Scope = scope;
    }
    
    public string PermissionKey => $"{Resource}:{Action}:{Scope}";
}