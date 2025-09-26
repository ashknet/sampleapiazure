using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class PatientContact : BaseEntity
{
    public Guid PatientId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Relationship { get; private set; } = string.Empty; // e.g., "Spouse", "Parent", "Emergency"
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public bool IsEmergencyContact { get; private set; }
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public virtual Patient Patient { get; private set; } = null!;
    
    protected PatientContact() { }
    
    public PatientContact(Guid patientId, string firstName, string lastName, string relationship, string? phone, string? email)
    {
        PatientId = patientId;
        FirstName = firstName;
        LastName = lastName;
        Relationship = relationship;
        Phone = phone;
        Email = email;
        IsActive = true;
        IsEmergencyContact = relationship.Contains("Emergency", StringComparison.OrdinalIgnoreCase);
    }
    
    public void UpdateContactInfo(string? phone, string? email)
    {
        Phone = phone;
        Email = email;
    }
    
    public void SetAsEmergencyContact(bool isEmergency)
    {
        IsEmergencyContact = isEmergency;
    }
    
    public void Deactivate() => IsActive = false;
}