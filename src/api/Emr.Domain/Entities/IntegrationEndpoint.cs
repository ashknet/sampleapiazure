using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class IntegrationEndpoint : BaseEntity
{
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty; // e.g., "FHIR", "HL7v2", "X12", "API"
    public string Direction { get; private set; } = string.Empty; // e.g., "Inbound", "Outbound", "Bidirectional"
    public string EndpointUrl { get; private set; } = string.Empty;
    public string? AuthType { get; private set; } // e.g., "OAuth2", "ApiKey", "Certificate"
    public string? ConfigurationJson { get; private set; } // Encrypted configuration details
    public bool IsActive { get; private set; }
    public DateTime? LastSuccessfulConnection { get; private set; }
    public string? LastError { get; private set; }
    public DateTime? LastErrorAt { get; private set; }
    
    // Navigation properties
    public virtual Organization Organization { get; private set; } = null!;
    
    protected IntegrationEndpoint() { }
    
    public IntegrationEndpoint(
        Guid organizationId,
        string name,
        string type,
        string direction,
        string endpointUrl)
    {
        OrganizationId = organizationId;
        Name = name;
        Type = type;
        Direction = direction;
        EndpointUrl = endpointUrl;
        IsActive = true;
    }
    
    public void UpdateConfiguration(string authType, string configurationJson)
    {
        AuthType = authType;
        ConfigurationJson = configurationJson;
    }
    
    public void RecordSuccess()
    {
        LastSuccessfulConnection = DateTime.UtcNow;
        LastError = null;
        LastErrorAt = null;
    }
    
    public void RecordError(string error)
    {
        LastError = error;
        LastErrorAt = DateTime.UtcNow;
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}