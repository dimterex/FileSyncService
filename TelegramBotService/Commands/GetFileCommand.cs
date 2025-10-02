namespace TelegramBotService.Commands;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using _Interfaces_;

using Core.Process;

using HtmlAgilityPack;

using Telegram.Bot;
using Telegram.Bot.Types;

using File = System.IO.File;

public class GetFileCommand : ITelegramCommand
{
    private readonly string _folderToSave;
    private const int ChunkSize = 8192;
    public GetFileCommand(string folderToSave)
    {
        _folderToSave = folderToSave;
    }

    private async Task DownloadFileAsync(string url, string fileName, string outputPath, Action<string> notifyStatus)
    {
       
    }
    

    public async void Handle(ITelegramBotClient botClient, long chatId, int replyToMessageId, string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return;

        var host = new Uri(url);

        var outputPath = Path.Join(_folderToSave, host.Host);
        if (Directory.Exists(outputPath))
            Directory.Delete(outputPath, true);
        
        var zipName = DateTime.Now.ToFileTime() + ".zip";
        var zipPath = Path.Join(_folderToSave, zipName);
        if (File.Exists(zipPath))
            File.Delete(zipPath);
        
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        
        var message = await botClient.SendTextMessageAsync(chatId, "Downloading", replyToMessageId: replyToMessageId);
        async Task<Message> NotifyStatus(string text)
        {
            return await botClient.EditMessageTextAsync(chatId, message.MessageId, text);
        }

        var path = Path.Join(outputPath, host.Segments[^1]);
        using (var httpClient = new HttpClient())
        {
            try
            {
                using (var response = await httpClient.GetStreamAsync(url))
                {
                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var buffer = new byte[8192]; // размер буфера для копирования данных
                        int bytesRead;
                        while ((bytesRead = await response.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await NotifyStatus($"An error occurred: {ex.Message}");
                return;
            }
        }
        
        await NotifyStatus( "Packaging");
        ZipFile.CreateFromDirectory(outputPath, zipPath);
        
        Directory.Delete(outputPath, true);

        await NotifyStatus("Sending");
        using (var fileStream = File.OpenRead(zipPath))
        {
            var file = new InputFileStream(fileStream,  host.Host + ".zip");
            await botClient.SendDocumentAsync(chatId, file, replyToMessageId: replyToMessageId);
            await botClient.DeleteMessageAsync(chatId, message.MessageId);
        }

        File.Delete(zipPath);
    }

}
