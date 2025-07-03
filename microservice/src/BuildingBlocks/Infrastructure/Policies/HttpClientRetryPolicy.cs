using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;

namespace Infrastructure.Policies;

public static class HttpClientRetryPolicy
{
    public static IHttpClientBuilder UseImmediateHttpRetryPolicy(this IHttpClientBuilder builder, int retryCount = 3)
    {
        return builder.AddPolicyHandler(ConfigureImmediateHttpRetry(retryCount));
    }

    public static IHttpClientBuilder UseLinearHttpRetryPolicy(this IHttpClientBuilder builder, int retryCount = 3,
        int fromSeconds = 3)
    {
        return builder.AddPolicyHandler(ConfigureLinearHttpRetry(retryCount, fromSeconds));
    }

    public static IHttpClientBuilder UseExponentialHttpRetryPolicy(this IHttpClientBuilder builder, int retryCount = 5)
    {
        return builder.AddPolicyHandler(ConfigureExponentialHttpRetry(retryCount));
    }

    public static IHttpClientBuilder UseCircuitBreakerPolicy(this IHttpClientBuilder builder,
        int eventsBeforeBreaking = 3, int fromSeconds = 30)
    {
        return builder.AddPolicyHandler(ConfigureCircuitBreakerPolicy(eventsBeforeBreaking, fromSeconds));
    }

    public static IHttpClientBuilder ConfigureTimeOutPolicy(this IHttpClientBuilder builder, int second = 5)
    {
        return builder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(second));
    }


    private static IAsyncPolicy<HttpResponseMessage> ConfigureImmediateHttpRetry(int retryCount)
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .RetryAsync(retryCount,
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        "Retry {RetryCount} of {ContextPolicyKey} at {ContextOperationKey} due to {DelegateResult}.",
                        retryCount, context.PolicyKey, context.OperationKey, exception.Exception.Message);
                });

    private static IAsyncPolicy<HttpResponseMessage> ConfigureLinearHttpRetry(int retryCount, int fromSeconds)
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(retryCount,
                retryAttemp => TimeSpan.FromSeconds(fromSeconds),
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        "Retry {RetryCount} of {ContextPolicyKey} at {ContextOperationKey} due to {DelegateResult}.",
                        retryCount, context.PolicyKey, context.OperationKey, exception.Exception.Message);
                });

    private static IAsyncPolicy<HttpResponseMessage> ConfigureExponentialHttpRetry(int retryCount)
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(retryCount,
                retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
                (exception, retryCount, context) =>
                {
                    Log.Error(
                        "Retry {RetryCount} of {ContextPolicyKey} at {ContextOperationKey} due to {DelegateResult}.",
                        retryCount, context.PolicyKey, context.OperationKey, exception.Exception.Message);
                });

    private static IAsyncPolicy<HttpResponseMessage> ConfigureCircuitBreakerPolicy(int eventsBeforeBreaking,
        int fromSeconds)
        => HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: eventsBeforeBreaking,
                durationOfBreak: TimeSpan.FromSeconds(fromSeconds));
}