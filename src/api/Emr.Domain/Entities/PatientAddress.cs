using Emr.Domain.Common;
using Emr.Domain.ValueObjects;

namespace Emr.Domain.Entities;

public class PatientAddress : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Address Address { get; private set; } = null!;
    public string Type { get; private set; } = string.Empty; // e.g., "Home", "Work", "Temporary"
    public bool IsPrimary { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    
    // Navigation properties
    public virtual Patient Patient { get; private set; } = null!;
    
    protected PatientAddress() { }
    
    public PatientAddress(Guid patientId, Address address, string type, bool isPrimary)
    {
        PatientId = patientId;
        Address = address;
        Type = type;
        IsPrimary = isPrimary;
        IsActive = true;
        EffectiveFrom = DateTime.UtcNow;
    }
    
    public void UpdateAddress(Address address)
    {
        Address = address;
    }
    
    public void SetAsPrimary()
    {
        IsPrimary = true;
    }
    
    public void UnsetPrimary()
    {
        IsPrimary = false;
    }
    
    public void Deactivate(DateTime? effectiveTo = null)
    {
        IsActive = false;
        EffectiveTo = effectiveTo ?? DateTime.UtcNow;
    }
}