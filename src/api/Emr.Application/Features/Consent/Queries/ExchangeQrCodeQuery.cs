using MediatR;
using Emr.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Emr.Application.Features.Consent.Queries;

public class ExchangeQrCodeQuery : IRequest<ExchangeQrCodeResponse>
{
    public string Code { get; set; } = string.Empty;
}

public class ExchangeQrCodeResponse
{
    public string? AccessToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public List<string> Scopes { get; set; } = new();
    public PatientInfo? Patient { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class PatientInfo
{
    public Guid PatientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string MedicalRecordNumber { get; set; } = string.Empty;
}

public class ExchangeQrCodeQueryHandler : IRequestHandler<ExchangeQrCodeQuery, ExchangeQrCodeResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;
    private readonly ICurrentUserService _currentUser;

    public ExchangeQrCodeQueryHandler(
        IApplicationDbContext context,
        IDateTime dateTime,
        ICurrentUserService currentUser)
    {
        _context = context;
        _dateTime = dateTime;
        _currentUser = currentUser;
    }

    public async Task<ExchangeQrCodeResponse> Handle(ExchangeQrCodeQuery request, CancellationToken cancellationToken)
    {
        // Get the share token with related data
        var shareToken = await _context.ShareTokens
            .Include(st => st.Patient)
                .ThenInclude(p => p!.User)
            .Include(st => st.Organization)
            .FirstOrDefaultAsync(st => st.Code == request.Code, cancellationToken);

        if (shareToken == null)
        {
            return new ExchangeQrCodeResponse
            {
                Success = false,
                Message = "Invalid code"
            };
        }

        if (!shareToken.IsValid)
        {
            return new ExchangeQrCodeResponse
            {
                Success = false,
                Message = "Code is not valid or has expired"
            };
        }

        // Verify the requesting user belongs to the organization
        var userOrgAssignment = await _context.UserOrganizationAssignments
            .AnyAsync(uoa => 
                uoa.UserId.ToString() == _currentUser.UserId && 
                uoa.OrganizationId == shareToken.OrganizationId &&
                uoa.IsActive, 
                cancellationToken);

        if (!userOrgAssignment)
        {
            return new ExchangeQrCodeResponse
            {
                Success = false,
                Message = "Unauthorized organization access"
            };
        }

        // Generate access token
        var accessToken = GenerateAccessToken();
        var expiresAt = _dateTime.UtcNow.AddHours(1); // Short-lived token

        // Create connection link
        var connectionLink = new Domain.Entities.ConnectionLink(
            shareToken.Id,
            accessToken,
            expiresAt,
            _currentUser.IpAddress,
            _currentUser.UserAgent);

        _context.ConnectionLinks.Add(connectionLink);
        
        // Record access
        shareToken.RecordAccess();
        
        await _context.SaveChangesAsync(cancellationToken);

        // Get authorized scopes
        var scopes = string.IsNullOrEmpty(shareToken.AuthorizedScopes) 
            ? new List<string>() 
            : System.Text.Json.JsonSerializer.Deserialize<List<string>>(shareToken.AuthorizedScopes) ?? new List<string>();

        return new ExchangeQrCodeResponse
        {
            Success = true,
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            Scopes = scopes,
            Patient = shareToken.Patient != null ? new PatientInfo
            {
                PatientId = shareToken.Patient.Id,
                FirstName = shareToken.Patient.User.FirstName,
                LastName = shareToken.Patient.User.LastName,
                DateOfBirth = shareToken.Patient.User.DateOfBirth,
                MedicalRecordNumber = shareToken.Patient.MedicalRecordNumber
            } : null
        };
    }

    private static string GenerateAccessToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}