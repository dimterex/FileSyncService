namespace PublicProject.Actions
{
    using System.Collections.Generic;

    using _Interfaces_;

    using NLog;

    using ServicesApi.Common._Interfaces_;
    using ServicesApi.History;

    using HistoryDto = Common.DatabaseProject.Dto.HistoryDto;

    public class GetHistoryAction : IMessageHandler<GetHistoryRequest>
    {
        private const string TAG = nameof(GetHistoryAction);
        private readonly IHistoryService _historyService;
        private readonly ILogger _logger;

        public GetHistoryAction(IHistoryService historyService)
        {
            _historyService = historyService;
            _logger = LogManager.GetLogger(TAG);
        }
        public IMessage Handler(GetHistoryRequest message)
        {
            var response = new GetHistoryResponse();

            IList<HistoryDto> events = _historyService.GetEvents();

            foreach (HistoryDto hist in events)
            {
                response.Items.Add(
                    new ServicesApi.History.HistoryDto
                    {
                        Id = hist.Id,
                        Action = hist.Action,
                        Login = hist.Login,
                        TimeStamp = hist.TimeStamp,
                        File = hist.File
                    });
            }

            return response;
        }
    }
}
