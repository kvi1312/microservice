using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ordering.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _stopwatch;
    private readonly ILogger<TRequest> _logger;

    public PerformanceBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
        _stopwatch = new Stopwatch();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _stopwatch.Start();
        var response = await next();
        _stopwatch.Stop();

        var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

        if (elapsedMilliseconds <= 500) return response;

        var requestName = typeof(TRequest).Name;
        _logger.LogWarning(
            "[PERFORMANCE] Request taking too much time : {Name} - {ElapsedMilliseconds} milliseconds - {@Request}",
            requestName, elapsedMilliseconds, request);

        return response;
    }
}