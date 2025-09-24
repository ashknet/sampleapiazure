using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class ConnectionLink : BaseEntity
{
    public Guid ShareTokenId { get; private set; }
    public string AccessToken { get; private set; } = string.Empty;
    public DateTime IssuedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public bool IsRevoked { get; private set; }
    
    // Navigation properties
    public virtual ShareToken ShareToken { get; private set; } = null!;
    
    protected ConnectionLink() { }
    
    public ConnectionLink(Guid shareTokenId, string accessToken, DateTime expiresAt, string? ipAddress = null, string? userAgent = null)
    {
        ShareTokenId = shareTokenId;
        AccessToken = accessToken;
        IssuedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }
    
    public void Revoke()
    {
        IsRevoked = true;
    }
    
    public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}