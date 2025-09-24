using System.Text.Json;
using Emr.Application.Common.Interfaces;
using Emr.Domain.Entities;

namespace Emr.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;

    public AuditService(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task LogAsync(
        string action,
        string resource,
        string resourceId,
        bool success = true,
        string? failureReason = null,
        string? patientId = null,
        string? consentId = null,
        string? purposeOfUse = null,
        Dictionary<string, object>? additionalData = null,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog(
            _currentUser.UserId ?? "anonymous",
            _currentUser.UserName ?? "Anonymous",
            string.Join(",", _currentUser.Roles),
            action,
            resource,
            resourceId,
            _currentUser.IpAddress ?? "unknown",
            success);

        auditLog.SetPatientContext(patientId ?? string.Empty);
        auditLog.SetOrganizationContext(_currentUser.OrganizationId ?? string.Empty);

        if (!string.IsNullOrEmpty(consentId))
        {
            auditLog.SetConsentContext(consentId, purposeOfUse ?? string.Empty);
        }

        if (!success && !string.IsNullOrEmpty(failureReason))
        {
            auditLog.SetFailure(failureReason);
        }

        if (additionalData != null && additionalData.Any())
        {
            auditLog.SetAdditionalData(JsonSerializer.Serialize(additionalData));
        }

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AuditLog> CreateAuditLogAsync(
        string action,
        string resource,
        string resourceId,
        bool success = true,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog(
            _currentUser.UserId ?? "anonymous",
            _currentUser.UserName ?? "Anonymous",
            string.Join(",", _currentUser.Roles),
            action,
            resource,
            resourceId,
            _currentUser.IpAddress ?? "unknown",
            success);

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync(cancellationToken);

        return auditLog;
    }
}