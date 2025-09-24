using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class PatientIdentifier : BaseEntity
{
    public Guid PatientId { get; private set; }
    public string System { get; private set; } = string.Empty; // e.g., "SSN", "DL", "Passport"
    public string Value { get; private set; } = string.Empty;
    public string? Type { get; private set; } // Additional categorization
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public virtual Patient Patient { get; private set; } = null!;
    
    protected PatientIdentifier() { }
    
    public PatientIdentifier(Guid patientId, string system, string value, string? type = null)
    {
        PatientId = patientId;
        System = system;
        Value = value;
        Type = type;
        IsActive = true;
    }
    
    public void Deactivate() => IsActive = false;
}