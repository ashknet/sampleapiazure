using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class User : BaseEntity
{
    public string ExternalId { get; private set; } = string.Empty; // Azure AD B2C object ID
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }
    public string UserType { get; private set; } = string.Empty; // Patient, Staff, Service
    public string? PhoneNumber { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    
    // Staff-specific properties
    public string? NpiNumber { get; private set; }
    public string? LicenseNumber { get; private set; }
    public string? Specialty { get; private set; }
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual ICollection<UserOrganizationAssignment> OrganizationAssignments { get; private set; } = new List<UserOrganizationAssignment>();
    public virtual Patient? PatientProfile { get; private set; }
    
    protected User() { }
    
    public User(string externalId, string email, string firstName, string lastName, string userType)
    {
        ExternalId = externalId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        UserType = userType;
        IsActive = true;
    }
    
    public void UpdatePersonalInfo(string firstName, string lastName, string? middleName, string? phoneNumber, DateTime? dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
    }
    
    public void UpdateStaffInfo(string? npiNumber, string? licenseNumber, string? specialty)
    {
        if (UserType != "Staff")
            throw new InvalidOperationException("Staff information can only be set for staff users");
            
        NpiNumber = npiNumber;
        LicenseNumber = licenseNumber;
        Specialty = specialty;
    }
    
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    
    public string FullName => $"{FirstName} {(string.IsNullOrEmpty(MiddleName) ? "" : MiddleName + " ")}{LastName}".Trim();
}