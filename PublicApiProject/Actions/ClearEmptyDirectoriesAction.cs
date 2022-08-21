using System;
using System.Text;
using Core.Publisher;
using Core.Publisher._Interfaces_;
using FileSystemProject;
using ServicesApi.Common._Interfaces_;
using ServicesApi.FileStorage;
using ServicesApi.Telegram;

namespace PublicProject.Actions
{
    public class ClearEmptyDirectoriesAction : IMessageHandler<ClearEmptyDirectories>
    {
        private readonly IFileManager _fileManager;
        private readonly IPublisherService _publisherController;

        public ClearEmptyDirectoriesAction(IPublisherService publisherController, IFileManager fileManager)
        {
            _publisherController = publisherController;
            _fileManager = fileManager;
        }

        public void Handler(ClearEmptyDirectories message)
        {
            var removedList = _fileManager.RemoveEmptyDirectories(message.FilePaths);
            var sb = new StringBuilder();
            sb.AppendLine("Removed dictionaries:");
            sb.AppendJoin(Environment.NewLine, removedList);

            _publisherController.SendMessage(new TelegramMessage
            {
                Message = sb.ToString()
            });
        }
    }
}