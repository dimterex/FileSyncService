using System.Collections.Concurrent;
using System.Threading;

namespace Core.Promise
{
    public class PromiseService : IPromiseService
    {
        private readonly ConcurrentDictionary<int, Promise> _promises;
        private int _promiseCounter;

        public PromiseService()
        {
            _promiseCounter = 1;
            _promises = new ConcurrentDictionary<int, Promise>();
        }

        public Promise Get(int promiseId)
        {
            if (_promises.TryGetValue(promiseId, out var promise))
                return promise;

            return null;
        }

        public void Remove(int promiseId)
        {
            _promises.TryRemove(promiseId, out _);
        }

        public Promise Create(IPromisedMessage message)
        {
            var promise = new Promise();
            message.PromiseId = Interlocked.Increment(ref _promiseCounter);
            _promises[message.PromiseId] = promise;
            return promise;
        }
}