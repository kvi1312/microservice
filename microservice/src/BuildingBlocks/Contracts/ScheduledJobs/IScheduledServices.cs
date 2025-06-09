using System.Linq.Expressions;

namespace Contracts.ScheduledJobs;

public interface IScheduledServices
{
    #region Fire and Forget
    string Enqueue(Expression<Action> callback);
    string Enqueue<T>(Expression<Action<T>> callback);
    #endregion

    #region Delayed
    string Schedule(Expression<Action> callback, TimeSpan delay);
    string Schedule<T>(Expression<Action<T>> callback, TimeSpan delay);
    string Schedule(Expression<Action> callback, DateTimeOffset enqueueAt); // run at exactly this time
    #endregion

    #region Continuous
    string ContinueQueueWith(string parentJobId, Expression<Action> callback);
    #endregion

    bool Delete(string jobId);
    bool Requeue(string jobId);

}