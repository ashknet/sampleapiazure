using MediatR;
using Emr.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Emr.Application.Features.Consent.Commands;

public class AuthorizeQrCodeCommand : IRequest<AuthorizeQrCodeResponse>
{
    public string Code { get; set; } = string.Empty;
    public List<string> AuthorizedScopes { get; set; } = new();
    public int ConsentDurationHours { get; set; } = 24;
}

public class AuthorizeQrCodeResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class AuthorizeQrCodeCommandHandler : IRequestHandler<AuthorizeQrCodeCommand, AuthorizeQrCodeResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;

    public AuthorizeQrCodeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<AuthorizeQrCodeResponse> Handle(AuthorizeQrCodeCommand request, CancellationToken cancellationToken)
    {
        // Get the share token
        var shareToken = await _context.ShareTokens
            .Include(st => st.Organization)
            .FirstOrDefaultAsync(st => st.Code == request.Code, cancellationToken);

        if (shareToken == null)
        {
            return new AuthorizeQrCodeResponse
            {
                Success = false,
                Message = "Invalid code"
            };
        }

        if (shareToken.Status != "Pending")
        {
            return new AuthorizeQrCodeResponse
            {
                Success = false,
                Message = "Code has already been used"
            };
        }

        if (shareToken.ExpiresAt < _dateTime.UtcNow)
        {
            shareToken.Expire();
            await _context.SaveChangesAsync(cancellationToken);
            
            return new AuthorizeQrCodeResponse
            {
                Success = false,
                Message = "Code has expired"
            };
        }

        // Get current user's patient record
        var user = await _context.Users
            .Include(u => u.PatientProfile)
            .FirstOrDefaultAsync(u => u.ExternalId == _currentUser.UserId, cancellationToken);

        if (user?.PatientProfile == null)
        {
            return new AuthorizeQrCodeResponse
            {
                Success = false,
                Message = "User does not have a patient profile"
            };
        }

        // Authorize the token
        shareToken.Authorize(
            user.PatientProfile.Id,
            user.FullName,
            JsonSerializer.Serialize(request.AuthorizedScopes));

        // Create consent record
        var consentScopes = new
        {
            ReadScopes = request.AuthorizedScopes.Where(s => s.StartsWith("read:")).ToList(),
            WriteScopes = request.AuthorizedScopes.Where(s => s.StartsWith("write:")).ToList()
        };

        var consent = new Domain.Entities.Consent(
            user.PatientProfile.Id,
            "Disclosure",
            _dateTime.UtcNow,
            JsonSerializer.Serialize(consentScopes),
            shareToken.OrganizationId);

        consent.SetExpiration(_dateTime.UtcNow.AddHours(request.ConsentDurationHours));

        _context.Consents.Add(consent);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthorizeQrCodeResponse
        {
            Success = true,
            Message = "Authorization successful"
        };
    }
}