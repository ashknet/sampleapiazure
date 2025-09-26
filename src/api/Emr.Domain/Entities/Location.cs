using Emr.Domain.Common;
using Emr.Domain.ValueObjects;

namespace Emr.Domain.Entities;

public class Location : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public Address Address { get; private set; } = null!;
    public string? Phone { get; private set; }
    public string? Fax { get; private set; }
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public virtual Organization Organization { get; private set; } = null!;
    public virtual ICollection<Department> Departments { get; private set; } = new List<Department>();
    
    protected Location() { }
    
    public Location(Guid organizationId, string name, string code, Address address)
    {
        OrganizationId = organizationId;
        Name = name;
        Code = code;
        Address = address;
        IsActive = true;
    }
    
    public void UpdateDetails(string name, string? phone, string? fax)
    {
        Name = name;
        Phone = phone;
        Fax = fax;
    }
    
    public void UpdateAddress(Address address)
    {
        Address = address;
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}