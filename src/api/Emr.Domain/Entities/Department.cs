using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class Department : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid? LocationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? Specialty { get; private set; }
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public virtual Organization Organization { get; private set; } = null!;
    public virtual Location? Location { get; private set; }
    
    protected Department() { }
    
    public Department(Guid organizationId, string name, string code, Guid? locationId = null)
    {
        OrganizationId = organizationId;
        Name = name;
        Code = code;
        LocationId = locationId;
        IsActive = true;
    }
    
    public void UpdateDetails(string name, string? specialty)
    {
        Name = name;
        Specialty = specialty;
    }
    
    public void AssignToLocation(Guid locationId)
    {
        LocationId = locationId;
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}