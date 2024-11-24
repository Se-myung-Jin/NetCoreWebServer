namespace WebServerCore
{
    public class TaskTimer
    {
        private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        
        public Func<Task> Function { get; }
        public Action Action { get; }
        public TimeSpan DueTime { get; } = TimeSpan.Zero;
        public TimeSpan Period { get; } = TimeSpan.MaxValue;
        private CancellationTokenSource token;

        public TaskTimer(Func<Task> funcion, int dueTime, int period)
        {
            Function = funcion;
            DueTime = TimeSpan.FromMilliseconds(dueTime);
            Period = TimeSpan.FromMilliseconds(period);
        }
        
        public TaskTimer(Func<Task> function, TimeSpan dueTime, TimeSpan period)
        {
            Function = function;
            DueTime = dueTime;
            Period = period;
        }
        
        public TaskTimer(Action action, int dueTime, int period)
        {
            Action = action;
            DueTime = TimeSpan.FromMilliseconds(dueTime);
            Period = TimeSpan.FromMilliseconds(period);
        }
        
        public TaskTimer(Action action, TimeSpan dueTime, TimeSpan period)
        {
            Action = action;
            DueTime = dueTime;
            Period = period;
        }

        public void Start()
        {
            Stop();
            token = new CancellationTokenSource();
            Run();
        }
        
        public void Start(int cancellationDelay)
        {
            Stop();
            token = new CancellationTokenSource(cancellationDelay);
            Run();
        }

        public void Start(TimeSpan cancellationDelay)
        {
            Stop();
            token = new CancellationTokenSource(cancellationDelay);
            Run();
        }

        private void Stop()
        {
            if (token != null)
            {
                token.Cancel();
                token.Dispose();
                token = null;
            }
        }

        private void Run()
        {
            if (Function != null)
            {
                Task.Run(async () => await RunAsync(Function, DueTime, Period, token.Token));
            }
            else
            {
                Task.Run(() => RunAsync(Action, DueTime, Period, token.Token));
            }
        }

        private static async Task RunAsync(Func<Task> func, TimeSpan dueTime, TimeSpan period, CancellationToken token)
        {
            try
            {
                await Task.Delay(dueTime, token);
                
                if (token.IsCancellationRequested == false)
                {
                    await func();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            if (period != TimeSpan.Zero)
            {
                while (token.IsCancellationRequested == false)
                {
                    try
                    {
                        await Task.Delay(period, token);

                        await func();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
        }

        private static async Task RunAsync(Action action, TimeSpan dueTime, TimeSpan period, CancellationToken token)
        {
            try
            {
                await Task.Delay(dueTime, token);

                if (token.IsCancellationRequested == false)
                {
                    await Task.Run(() => action, token);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            if (period != TimeSpan.Zero)
            {
                while (token.IsCancellationRequested == false)
                {
                    try
                    {
                        await Task.Delay(period, token);

                        await Task.Run(() => action, token);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
        }
    }
}