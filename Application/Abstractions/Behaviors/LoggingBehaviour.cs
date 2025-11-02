using Application.Contracts;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUser _user;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentUser user)
    {
        _logger = logger;
        _user = user;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userName = _user.UserName ?? string.Empty;

        _logger.LogInformation("Starting Request: {Name} by {@UserName}", requestName, userName);

        var result = await next();

        if (result.IsFailure)
        {
            if (result.Errors is not null)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogWarning("Request Failed: {Name} by {@UserName} {@Request} with {@Error}", requestName, userName, request, error);
                }
            }
            else
            {
                _logger.LogWarning("Request Failed: {Name} by {@UserName}", requestName, userName);
            }
        }
        else
        {
            _logger.LogInformation("Completed Request: {Name} by {@UserName}", requestName, userName);
        }
        return result;
    }
}