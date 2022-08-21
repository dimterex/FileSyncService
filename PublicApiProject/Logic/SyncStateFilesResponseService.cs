using System.Collections.Concurrent;
using Core.Logger;
using Core.Logger._Enums_;
using Core.Logger._Interfaces_;
using PublicProject._Interfaces_;
using SdkProject.Api.Sync;

namespace PublicProject.Logic
{
    public class SyncStateFilesResponseService : ISyncStateFilesResponseService
    {
        private const string TAG = nameof(SyncStateFilesResponseService);
        private readonly ILoggerService _loggerService;
        private readonly ConcurrentDictionary<string, SyncStateFilesResponse> _responses;

        public SyncStateFilesResponseService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
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
                _loggerService.SendLog(LogLevel.Warning, TAG, () => $"Removed older sync state for {login}");
        }

        public void Add(string login, string token, SyncStateFilesResponse response)
        {
            if (!_responses.TryAdd(token, response))
                _loggerService.SendLog(LogLevel.Warning, TAG, () => $"Couldn't add new sync state for {login}");
        }
    }
}