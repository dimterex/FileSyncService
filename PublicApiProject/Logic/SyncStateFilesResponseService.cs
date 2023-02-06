using System.Collections.Concurrent;
using NLog;
using PublicProject._Interfaces_;
using SdkProject.Api.Sync;

namespace PublicProject.Logic
{
    public class SyncStateFilesResponseService : ISyncStateFilesResponseService
    {
        private const string TAG = nameof(SyncStateFilesResponseService);
        private readonly ConcurrentDictionary<string, SyncStateFilesResponse> _responses;
        private readonly ILogger _logger;

        public SyncStateFilesResponseService()
        {
            _logger = LogManager.GetLogger(TAG);
            _responses = new ConcurrentDictionary<string, SyncStateFilesResponse>();
        }

        public SyncStateFilesResponse GetResponse(string token)
        {
            if (_responses.TryGetValue(token, out var stateFilesResponse))
                return stateFilesResponse;

            return null;
        }

        public void Remove(string login, string token)
        {
            if (_responses.TryRemove(token, out _))
                _logger.Warn(() => $"Removed older sync state for {login}");
        }

        public void Add(string login, string token, SyncStateFilesResponse response)
        {
            if (!_responses.TryAdd(token, response))
                _logger.Warn(() => $"Couldn't add new sync state for {login}");
        }
    }
}