using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class ShareToken : BaseEntity
{
    public Guid? PatientId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty; // e.g., "QR", "Link"
    public string Status { get; private set; } = string.Empty; // e.g., "Pending", "Authorized", "Expired", "Revoked"
    public DateTime ExpiresAt { get; private set; }
    public string? RequestedScopes { get; private set; } // JSON of requested permissions
    public string? AuthorizedScopes { get; private set; } // JSON of granted permissions
    public DateTime? AuthorizedAt { get; private set; }
    public string? AuthorizedBy { get; private set; }
    public DateTime? LastAccessedAt { get; private set; }
    public int AccessCount { get; private set; }
    
    // Navigation properties
    public virtual Patient? Patient { get; private set; }
    public virtual Organization Organization { get; private set; } = null!;
    public virtual ICollection<ConnectionLink> ConnectionLinks { get; private set; } = new List<ConnectionLink>();
    
    protected ShareToken() { }
    
    public ShareToken(Guid organizationId, string code, string type, DateTime expiresAt, string requestedScopes)
    {
        OrganizationId = organizationId;
        Code = code;
        Type = type;
        Status = "Pending";
        ExpiresAt = expiresAt;
        RequestedScopes = requestedScopes;
    }
    
    public void Authorize(Guid patientId, string authorizedBy, string authorizedScopes)
    {
        PatientId = patientId;
        Status = "Authorized";
        AuthorizedAt = DateTime.UtcNow;
        AuthorizedBy = authorizedBy;
        AuthorizedScopes = authorizedScopes;
    }
    
    public void RecordAccess()
    {
        LastAccessedAt = DateTime.UtcNow;
        AccessCount++;
    }
    
    public void Expire()
    {
        Status = "Expired";
    }
    
    public void Revoke()
    {
        Status = "Revoked";
    }
    
    public bool IsValid => Status == "Authorized" && ExpiresAt > DateTime.UtcNow;
}