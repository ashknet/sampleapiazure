using Emr.Domain.Entities;

namespace Emr.Application.Common.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        string action,
        string resource,
        string resourceId,
        bool success = true,
        string? failureReason = null,
        string? patientId = null,
        string? consentId = null,
        string? purposeOfUse = null,
        Dictionary<string, object>? additionalData = null,
        CancellationToken cancellationToken = default);
        
    Task<AuditLog> CreateAuditLogAsync(
        string action,
        string resource,
        string resourceId,
        bool success = true,
        CancellationToken cancellationToken = default);
}