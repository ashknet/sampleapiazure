using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Emr.Application.Features.Consent.Commands;
using Emr.Application.Features.Consent.Queries;
using Asp.Versioning;

namespace Emr.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/connect")]
public class ConnectController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConnectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Generate a QR code for patient connection
    /// </summary>
    [HttpPost("qr")]
    [Authorize(Roles = "Registrar,Clinician,OrgAdmin")]
    public async Task<ActionResult<CreateQrCodeResponse>> CreateQrCode([FromBody] CreateQrCodeCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Authorize a QR code connection (called by patient mobile app)
    /// </summary>
    [HttpPost("qr/{code}/authorize")]
    [Authorize(Roles = "Patient")]
    public async Task<ActionResult<AuthorizeQrCodeResponse>> AuthorizeQrCode(
        string code,
        [FromBody] AuthorizeQrCodeRequest request)
    {
        var command = new AuthorizeQrCodeCommand
        {
            Code = code,
            AuthorizedScopes = request.AuthorizedScopes,
            ConsentDurationHours = request.ConsentDurationHours
        };

        var result = await _mediator.Send(command);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(result);
    }

    /// <summary>
    /// Exchange QR code for access token (called by hospital system)
    /// </summary>
    [HttpGet("qr/{code}/exchange")]
    [Authorize(Roles = "Registrar,Clinician,OrgAdmin,Integration")]
    public async Task<ActionResult<ExchangeQrCodeResponse>> ExchangeQrCode(string code)
    {
        var query = new ExchangeQrCodeQuery { Code = code };
        var result = await _mediator.Send(query);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(result);
    }
}

public class AuthorizeQrCodeRequest
{
    public List<string> AuthorizedScopes { get; set; } = new();
    public int ConsentDurationHours { get; set; } = 24;
}