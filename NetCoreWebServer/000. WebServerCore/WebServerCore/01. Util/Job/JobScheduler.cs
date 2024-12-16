using System.Collections.Concurrent;

namespace WebServerCore
{
    public class JobScheduler : IDisposable
    {
        private readonly ConcurrentDictionary<Guid, Job> jobs = new();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly Task schedulerTask;

        public JobScheduler()
        {
            schedulerTask = Task.Run(() => SchedulerLoop(cancellationTokenSource.Token));
        }

        public Guid RegisterJob(Action action, int intervalMilliseconds)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (intervalMilliseconds <= 0) throw new ArgumentOutOfRangeException(nameof(intervalMilliseconds));

            var id = Guid.NewGuid();
            var job = new Job(action, intervalMilliseconds);

            jobs[id] = job;

            return id;
        }

        public bool UnregisterJob(Guid actionId)
        {
            return jobs.TryRemove(actionId, out _);
        }

        private async Task SchedulerLoop(CancellationToken cancellationToken)
        {
            var delayTimes = new List<int>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                delayTimes.Clear();

                foreach (var keyValueJob in jobs.ToArray())
                {
                    var scheduledJob = keyValueJob.Value;

                    if (now >= scheduledJob.NextExecutionTime)
                    {
                        try
                        {
                            scheduledJob.Execute();
                        }
                        catch (Exception ex)
                        {
                            
                        }

                        scheduledJob.UpdateNextExecutionTime();
                    }

                    var delay = (int)(scheduledJob.NextExecutionTime - now).TotalMilliseconds;
                    if (delay > 0) delayTimes.Add(delay);
                }

                var minDelay = delayTimes.Count > 0 ? delayTimes.Min() : 5000;
                await Task.Delay(minDelay, cancellationToken);
            }
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            schedulerTask.Wait();
            cancellationTokenSource.Dispose();
        }
    }
}
