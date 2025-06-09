using Contracts.ScheduledJobs;
using System.Linq.Expressions;
using Hangfire;
namespace Infrastructure.ScheduledJobs;

public class HangfireServices : IScheduledServices
{
    public string ContinueQueueWith(string parentJobId, Expression<Action> callback) => BackgroundJob.ContinueJobWith(parentJobId, callback);

    public bool Delete(string jobId) => BackgroundJob.Delete(jobId);

    public string Enqueue(Expression<Action> callback) => BackgroundJob.Enqueue(callback);

    public string Enqueue<T>(Expression<Action<T>> callback) => BackgroundJob.Enqueue<T>(callback);

    public bool Requeue(string jobId) => BackgroundJob.Requeue(jobId);

    public string Schedule(Expression<Action> callback, TimeSpan delay) => BackgroundJob.Schedule(callback, delay);

    public string Schedule<T>(Expression<Action<T>> callback, TimeSpan delay) => BackgroundJob.Schedule<T>(callback, delay);

    public string Schedule(Expression<Action> callback, DateTimeOffset enqueueAt) => BackgroundJob.Schedule(callback, enqueueAt);
}
