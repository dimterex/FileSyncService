namespace Core.Daemon
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using NLog;

    public class Daemon
    {
        private readonly AutoResetEvent _closing;
        private readonly ILogger _logger;

        public Daemon()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _closing = new AutoResetEvent(false);
        }

        public void Run(Action action)
        {
            _logger.Error(() => "Starting");
            var task = Task.Factory.StartNew(
                () =>
                {
                    action?.Invoke();
                });
            
            task.ContinueWith(
                t =>
                {
                    _logger.Error(() => "Failed");
                    task.Dispose();
                    Stop();
                }, TaskContinuationOptions.OnlyOnFaulted);

            Console.CancelKeyPress += OnExit;
            _logger.Error(() => "Started");
            _closing.WaitOne();
        }

        private void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            Stop();
        }

        private void Stop()
        {
            Console.CancelKeyPress -= OnExit;
            Console.WriteLine("Exit");
            _closing.Set();
        }
    }
}
