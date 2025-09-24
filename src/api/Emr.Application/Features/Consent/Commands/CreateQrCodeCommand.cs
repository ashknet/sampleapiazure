using MediatR;
using Emr.Application.Common.Interfaces;
using Emr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Emr.Application.Features.Consent.Commands;

public class CreateQrCodeCommand : IRequest<CreateQrCodeResponse>
{
    public Guid OrganizationId { get; set; }
    public List<string> RequestedScopes { get; set; } = new();
    public int ExpirationMinutes { get; set; } = 15;
}

public class CreateQrCodeResponse
{
    public string QrCodeUrl { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class CreateQrCodeCommandHandler : IRequestHandler<CreateQrCodeCommand, CreateQrCodeResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IQrCodeService _qrCodeService;
    private readonly IDateTime _dateTime;

    public CreateQrCodeCommandHandler(
        IApplicationDbContext context,
        IQrCodeService qrCodeService,
        IDateTime dateTime)
    {
        _context = context;
        _qrCodeService = qrCodeService;
        _dateTime = dateTime;
    }

    public async Task<CreateQrCodeResponse> Handle(CreateQrCodeCommand request, CancellationToken cancellationToken)
    {
        // Validate organization exists
        var organizationExists = await _context.Organizations
            .AnyAsync(o => o.Id == request.OrganizationId && o.IsActive, cancellationToken);

        if (!organizationExists)
        {
            throw new InvalidOperationException("Organization not found or inactive");
        }

        // Generate unique code
        var code = GenerateUniqueCode();
        var expiresAt = _dateTime.UtcNow.AddMinutes(request.ExpirationMinutes);

        // Create share token
        var shareToken = new ShareToken(
            request.OrganizationId,
            code,
            "QR",
            expiresAt,
            JsonSerializer.Serialize(request.RequestedScopes));

        _context.ShareTokens.Add(shareToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate QR code URL
        var qrData = $"emr://connect/{code}";
        var qrCodeUrl = _qrCodeService.GenerateQrCode(qrData);

        return new CreateQrCodeResponse
        {
            QrCodeUrl = qrCodeUrl,
            Code = code,
            ExpiresAt = expiresAt
        };
    }

    private static string GenerateUniqueCode()
    {
        // Generate a secure random code
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}