using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class Organization : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string TaxId { get; private set; } = string.Empty;
    public string? NpiNumber { get; private set; }
    public string Type { get; private set; } = string.Empty; // Hospital, Clinic, Practice, etc.
    public bool IsActive { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }
    public string? Website { get; private set; }
    
    // Navigation properties
    public virtual ICollection<Location> Locations { get; private set; } = new List<Location>();
    public virtual ICollection<Department> Departments { get; private set; } = new List<Department>();
    public virtual ICollection<UserOrganizationAssignment> UserAssignments { get; private set; } = new List<UserOrganizationAssignment>();
    
    protected Organization() { }
    
    public Organization(string name, string taxId, string type)
    {
        Name = name;
        TaxId = taxId;
        Type = type;
        IsActive = true;
    }
    
    public void UpdateDetails(string name, string? npiNumber, string? contactEmail, string? contactPhone, string? website)
    {
        Name = name;
        NpiNumber = npiNumber;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        Website = website;
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}