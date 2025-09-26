using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string UserId { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string UserRole { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty; // e.g., "Read", "Create", "Update", "Delete"
    public string Resource { get; private set; } = string.Empty; // e.g., "Patient", "Document", "Appointment"
    public string ResourceId { get; private set; } = string.Empty;
    public string? PatientId { get; private set; }
    public string? OrganizationId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;
    public string? UserAgent { get; private set; }
    public string? PurposeOfUse { get; private set; }
    public string? ConsentId { get; private set; }
    public bool Success { get; private set; }
    public string? FailureReason { get; private set; }
    public string? AdditionalData { get; private set; } // JSON for extra context
    
    protected AuditLog() { }
    
    public AuditLog(
        string userId,
        string userName,
        string userRole,
        string action,
        string resource,
        string resourceId,
        string ipAddress,
        bool success = true)
    {
        UserId = userId;
        UserName = userName;
        UserRole = userRole;
        Action = action;
        Resource = resource;
        ResourceId = resourceId;
        IpAddress = ipAddress;
        Timestamp = DateTime.UtcNow;
        Success = success;
    }
    
    public void SetPatientContext(string patientId)
    {
        PatientId = patientId;
    }
    
    public void SetOrganizationContext(string organizationId)
    {
        OrganizationId = organizationId;
    }
    
    public void SetConsentContext(string consentId, string purposeOfUse)
    {
        ConsentId = consentId;
        PurposeOfUse = purposeOfUse;
    }
    
    public void SetFailure(string failureReason)
    {
        Success = false;
        FailureReason = failureReason;
    }
    
    public void SetAdditionalData(string additionalData)
    {
        AdditionalData = additionalData;
    }
}