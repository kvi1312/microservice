using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Infrastructure.Policies;

public static class HttpClientRetryPolicy
{
    public static IHttpClientBuilder UseImmediateHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(ConfigureImmediateHttpRetry());
    }

    public static IHttpClientBuilder UseLinearHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(ConfigureLinearHttpRetry());
    }

    public static IHttpClientBuilder UseExponentialHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(ConfigureExponentialHttpRetry());
    }

    public static IHttpClientBuilder UseCircuitBreakerPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(ConfigureCircuitBreakerPolicy());
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureImmediateHttpRetry()
        => HttpPolicyExtensions.HandleTransientHttpError().RetryAsync(retryCount: 3,
            (exception, retryCount, context) =>
            {
                Log.Error("Retry {RetryCount} of {ContextPolicyKey} at {ContextOperationKey} due to {DelegateResult}.", retryCount, context.PolicyKey, context.OperationKey, exception.Exception.Message);
            });

    private static IAsyncPolicy<HttpResponseMessage> ConfigureLinearHttpRetry()
        => HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(retryCount: 3,
            retryAttemp => TimeSpan.FromSeconds(3),
            (exception, retryCount, context) =>
            {
                Log.Error("Retry {RetryCount} of {ContextPolicyKey} at {ContextOperationKey} due to {DelegateResult}.", retryCount, context.PolicyKey, context.OperationKey, exception.Exception.Message);
            });

    private static IAsyncPolicy<HttpResponseMessage> ConfigureExponentialHttpRetry()
        => HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(retryCount: 3,
            retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
            (exception, retryCount, context) =>
            {
                Log.Error("Retry {RetryCount} of {ContextPolicyKey} at {ContextOperationKey} due to {DelegateResult}.", retryCount, context.PolicyKey, context.OperationKey, exception.Exception.Message);
            });

    private static IAsyncPolicy<HttpResponseMessage> ConfigureCircuitBreakerPolicy()
        => HttpPolicyExtensions.HandleTransientHttpError()
                               .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(30));
}