using System;
using NLog;
using PublicProject._Interfaces_;
using SdkProject.Api.History;
using SdkProject.Api.Sync;

namespace PublicProject.Modules
{
    public class HistoryModule : BaseApiModule
    {
        
        private const string TAG = nameof(HistoryModule);
        private readonly IHistoryService _historyService;
        private readonly ILogger _logger;

        public HistoryModule(IApiController apiController, IHistoryService historyService)
            : base("history", new Version(0, 1), apiController)
        {
            _historyService = historyService;
            _logger = LogManager.GetLogger(TAG);
        }
        
        protected override void OnInitialize()
        {
            RegisterPostRequestWithBody<HistoryRequest>(OnHistoryRequest);
        }

        private void OnHistoryRequest(SyncFilesRequest fileAction, HistoryRequest request, HttpRequestEventModel e)
        {
            var response = new HistoryResponse();

            var events = _historyService.GetEvents();

            foreach (var hist in events)
            {
                response.Items.Add(new HistoryModel()
                {
                    Id = hist.Id,
                    Action = hist.Action,
                    Login = hist.Login,
                    TimeStamp = hist.TimeStamp,
                    File = hist.File,
                });
            }
            
            _apiController.SendResponse(e, response);
        }
    }
}