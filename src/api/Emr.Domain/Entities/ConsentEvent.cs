using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class ConsentEvent : BaseEntity
{
    public Guid ConsentId { get; private set; }
    public string EventType { get; private set; } = string.Empty; // e.g., "Created", "Updated", "Revoked", "Accessed"
    public string Description { get; private set; } = string.Empty;
    public string? PerformedBy { get; private set; }
    public DateTime OccurredAt { get; private set; }
    
    // Navigation properties
    public virtual Consent Consent { get; private set; } = null!;
    
    protected ConsentEvent() { }
    
    public ConsentEvent(Guid consentId, string eventType, string description, string? performedBy = null)
    {
        ConsentId = consentId;
        EventType = eventType;
        Description = description;
        PerformedBy = performedBy;
        OccurredAt = DateTime.UtcNow;
    }
}