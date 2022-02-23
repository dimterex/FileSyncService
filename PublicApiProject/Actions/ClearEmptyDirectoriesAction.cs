using System;
using System.Text;
using Core.Publisher;
using FileSystemProject;
using ServicesApi.Common._Interfaces_;
using ServicesApi.FileStorage;
using ServicesApi.Telegram;

namespace PublicProject.Actions
{
    public class ClearEmptyDirectoriesAction : IMessageHandler<ClearEmptyDirectories>
    {
        private readonly PublisherController _publisherController;
        private readonly IFileManager _fileManager;

        public ClearEmptyDirectoriesAction(PublisherController publisherController, IFileManager fileManager)
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
            
            _publisherController.Send(new TelegramMessage()
            {
                Message = sb.ToString()
            });
        }
    }
}