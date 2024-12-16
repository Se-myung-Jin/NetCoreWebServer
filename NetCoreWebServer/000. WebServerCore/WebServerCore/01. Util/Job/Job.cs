namespace WebServerCore
{
    public class Job
    {
        private readonly Action action;
        private readonly int intervalMilliseconds;

        public DateTime NextExecutionTime { get; private set; }

        public Job(Action action, int intervalMilliseconds)
        {
            this.action = action;
            this.intervalMilliseconds = intervalMilliseconds;
            NextExecutionTime = DateTime.UtcNow.AddMilliseconds(this.intervalMilliseconds);
        }

        public void Execute()
        {
            action?.Invoke();
        }

        public void UpdateNextExecutionTime()
        {
            NextExecutionTime = DateTime.UtcNow.AddMilliseconds(this.intervalMilliseconds);
        }
    }
}
