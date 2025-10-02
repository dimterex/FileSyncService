namespace PlaceholderService.Logic
{
    using System.Collections.Concurrent;

    using _Interfaces_;

    using NLog;

    using SdkProject.Api.Sync;

    public class SyncStateFilesResponseService : ISyncStateFilesResponseService
    {
        private const string TAG = nameof(SyncStateFilesResponseService);
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, SyncStateFilesResponse> _responses;

        public SyncStateFilesResponseService()
        {
            _logger = LogManager.GetLogger(TAG);
            _responses = new ConcurrentDictionary<string, SyncStateFilesResponse>();
        }

        public SyncStateFilesResponse GetResponse(string token)
        {
            if (_responses.TryGetValue(token, out SyncStateFilesResponse stateFilesResponse))
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
