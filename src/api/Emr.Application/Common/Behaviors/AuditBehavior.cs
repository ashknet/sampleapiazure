using MediatR;
using Emr.Application.Common.Interfaces;
using System.Text.Json;

namespace Emr.Application.Common.Behaviors;

public interface IAuditableRequest
{
    string Action { get; }
    string Resource { get; }
    string ResourceId { get; }
    string? PatientId { get; }
}

public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuditService _auditService;

    public AuditBehavior(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not IAuditableRequest auditableRequest)
        {
            return await next();
        }

        var success = true;
        string? failureReason = null;

        try
        {
            var response = await next();
            return response;
        }
        catch (Exception ex)
        {
            success = false;
            failureReason = ex.Message;
            throw;
        }
        finally
        {
            var additionalData = new Dictionary<string, object>
            {
                ["RequestType"] = typeof(TRequest).Name,
                ["RequestData"] = JsonSerializer.Serialize(request)
            };

            await _auditService.LogAsync(
                auditableRequest.Action,
                auditableRequest.Resource,
                auditableRequest.ResourceId,
                success,
                failureReason,
                auditableRequest.PatientId,
                additionalData: additionalData,
                cancellationToken: cancellationToken);
        }
    }
}