namespace PublicProject.Actions
{
    using System;
    using System.Collections.Generic;

    using _Interfaces_;

    using FileSystemProject;

    using ServicesApi.Common;
    using ServicesApi.Common._Interfaces_;
    using ServicesApi.FileStorage;

    public class ClearEmptyDirectoriesAction : IMessageHandler<ClearEmptyDirectoriesRequest>
    {
        private readonly IFileManager _fileManager;
        private readonly IHistoryService _historyService;

        public ClearEmptyDirectoriesAction(IFileManager fileManager, IHistoryService historyService)
        {
            _fileManager = fileManager;
            _historyService = historyService;
        }

        public IMessage Handler(ClearEmptyDirectoriesRequest message)
        {
            IList<string> removedList = _fileManager.RemoveEmptyDirectories(message.FilePaths);
            _historyService.AddNewEvent("root", string.Join(Environment.NewLine, removedList), "Removed dictionaries");

            return new StatusResponse
            {
                Message = $"Removed: {string.Join(Environment.NewLine, removedList)}",
                Status = Status.Ok
            };
        }
    }
}
