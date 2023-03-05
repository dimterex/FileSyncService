namespace Core.Daemon
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class Daemon
    {
        private readonly AutoResetEvent _closing;

        public Daemon()
        {
            _closing = new AutoResetEvent(false);
        }

        public void Run(Action action)
        {
            var task = Task.Factory.StartNew(
                () =>
                {
                    action?.Invoke();
                });
            
            task.ContinueWith(
                t =>
                {
                    task.Dispose();
                    Stop();
                }, TaskContinuationOptions.OnlyOnFaulted);
            

            Console.CancelKeyPress += OnExit;
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
