using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; }
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    
    protected Role() { }
    
    public Role(string name, string displayName, bool isSystemRole = false)
    {
        Name = name;
        DisplayName = displayName;
        IsSystemRole = isSystemRole;
    }
    
    public void UpdateDetails(string displayName, string? description)
    {
        if (IsSystemRole)
            throw new InvalidOperationException("System roles cannot be modified");
            
        DisplayName = displayName;
        Description = description;
    }
    
    // System role constants
    public const string Patient = "Patient";
    public const string Clinician = "Clinician";
    public const string Registrar = "Registrar";
    public const string HIM = "HIM"; // Health Information Management
    public const string OrgAdmin = "OrgAdmin";
    public const string Integration = "Integration";
    public const string Auditor = "Auditor";
}