using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class Consent : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Guid? OrganizationId { get; private set; }
    public string Type { get; private set; } = string.Empty; // e.g., "Treatment", "Disclosure", "Research"
    public string Status { get; private set; } = string.Empty; // e.g., "Active", "Revoked", "Expired"
    public DateTime ConsentDate { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public string? PurposeOfUse { get; private set; }
    public string? ConsentingParty { get; private set; } // If not the patient (e.g., guardian)
    public string? RevokedBy { get; private set; }
    public DateTime? RevokedDate { get; private set; }
    
    // Scope details (JSON serialized)
    public string ScopeJson { get; private set; } = string.Empty;
    
    // Navigation properties
    public virtual Patient Patient { get; private set; } = null!;
    public virtual Organization? Organization { get; private set; }
    public virtual ICollection<ConsentEvent> Events { get; private set; } = new List<ConsentEvent>();
    
    protected Consent() { }
    
    public Consent(Guid patientId, string type, DateTime consentDate, string scopeJson, Guid? organizationId = null)
    {
        PatientId = patientId;
        Type = type;
        ConsentDate = consentDate;
        Status = "Active";
        ScopeJson = scopeJson;
        OrganizationId = organizationId;
        
        AddEvent("Created", "Consent created");
    }
    
    public void SetExpiration(DateTime expirationDate)
    {
        ExpirationDate = expirationDate;
        AddEvent("Updated", $"Expiration date set to {expirationDate:yyyy-MM-dd}");
    }
    
    public void Revoke(string revokedBy)
    {
        Status = "Revoked";
        RevokedBy = revokedBy;
        RevokedDate = DateTime.UtcNow;
        AddEvent("Revoked", $"Consent revoked by {revokedBy}");
    }
    
    public void Expire()
    {
        Status = "Expired";
        AddEvent("Expired", "Consent expired");
    }
    
    public bool IsValid => Status == "Active" && (!ExpirationDate.HasValue || ExpirationDate.Value > DateTime.UtcNow);
    
    private void AddEvent(string eventType, string description)
    {
        var consentEvent = new ConsentEvent(Id, eventType, description);
        Events.Add(consentEvent);
    }
}