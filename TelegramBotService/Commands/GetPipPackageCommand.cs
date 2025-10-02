namespace TelegramBotService.Commands;

using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using _Interfaces_;

using Core.Process;

using Telegram.Bot;
using Telegram.Bot.Types;

using File = System.IO.File;

public class GetPipPackageCommand : ITelegramCommand
{
    private readonly IProcessService _processService;
    private readonly string _folderToSave;

    public GetPipPackageCommand(IProcessService processService, string folderToSave)
    {
        _processService = processService;
        _folderToSave = folderToSave;
    }
    
    public async void Handle(ITelegramBotClient botClient, long chatId, int replyToMessageId, string packageName)
    {
        if (string.IsNullOrWhiteSpace(packageName))
            return;
        
        var targetPath = Path.Join(_folderToSave, packageName);
        if (Directory.Exists(targetPath))
            Directory.Delete(targetPath, true);

        var zipPath = targetPath + ".zip";
        if (File.Exists(zipPath))
            File.Delete(zipPath);
        
        var message = await botClient.SendTextMessageAsync(chatId, $"Downloading {packageName}", replyToMessageId: replyToMessageId);
        
        async Task<Message> NotifyStatus(string text)
        {
            return await botClient.EditMessageTextAsync(chatId, message.MessageId, text);
        }
        
        _processService.StartProcess("pip", $"install --target={targetPath} {packageName}",  ( async s =>
                    {
                        await NotifyStatus(s);
                    }));

        await NotifyStatus("Packaging");
        ZipFile.CreateFromDirectory(targetPath, zipPath);
        
        Directory.Delete(targetPath, true);
        
        await NotifyStatus("Sending");
        using (var fileStream = File.OpenRead(zipPath))
        {
            var file = new InputFileStream(fileStream, $"{packageName}.zip");
            await botClient.SendDocumentAsync(chatId, file, replyToMessageId: replyToMessageId);
            await botClient.DeleteMessageAsync(chatId, message.MessageId);
        }
        
        File.Delete(zipPath);
    }
}
