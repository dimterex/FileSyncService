using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Daemon
{
    public class Daemon
    {
        private readonly AutoResetEvent _closing;

        public Daemon()
        {
            _closing = new AutoResetEvent(false);
        }

        public void Run(Action action)
        {
            Task.Factory.StartNew(() => { action?.Invoke(); });

            Console.CancelKeyPress += OnExit;
            _closing.WaitOne();
        }

        private void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            Console.CancelKeyPress -= OnExit;
            Console.WriteLine("Exit");
            _closing.Set();
        }
    }
}