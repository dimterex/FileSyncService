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

public class GetWebSiteCommand : ITelegramCommand
{
    private readonly string _folderToSave;

    public GetWebSiteCommand(string folderToSave)
    {
        _folderToSave = folderToSave;
    }

    private async Task<byte[]> GetBytesByUrl(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return Array.Empty<byte>();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            return bytes;
        }
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

        var rawContent = await GetBytesByUrl(url);
        string htmlContent = System.Text.Encoding.UTF8.GetString(rawContent);
        
        if (string.IsNullOrWhiteSpace(htmlContent))
        {
            await botClient.SendTextMessageAsync(chatId, "HTML Content is empty", replyToMessageId: replyToMessageId);
            return;
        }

        // Создаем экземпляр HtmlDocument и загружаем HTML-контент
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);

        var scripts = htmlDocument.DocumentNode.SelectNodes("//script");
        var message = await botClient.SendTextMessageAsync(chatId, "Download scripts", replyToMessageId: replyToMessageId);
        
        async Task<Message> NotifyStatus(string text)
        {
            return await botClient.EditMessageTextAsync(chatId, message.MessageId, text);
        }

        var link = $"{host.Scheme}://{host.Host}";
        
        await Convert(scripts, "src", "scripts", link, outputPath);
        var links = htmlDocument.DocumentNode.SelectNodes("//link");
        await NotifyStatus("Download links");
        await Convert(links, "href",  "styles", link, outputPath);
        
        var images = htmlDocument.DocumentNode.SelectNodes("//img");
        await NotifyStatus("Download images");
        await Convert(images, "src", "images", link, outputPath);
        
        var hrefs = htmlDocument.DocumentNode.SelectNodes("//a");
        await NotifyStatus("Download files");
        await Convert(hrefs, "href", "files", link, outputPath);

        // Сохраняем HTML-код страницы
        htmlDocument.Save(Path.Combine(outputPath, "index.html"));
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

    private async Task Convert(
        HtmlNodeCollection collection,
        string attribute,
        string directory,
        string url,
        string outputPath)
    {
        if (collection == null)
            return;

        var targetPath = Path.Combine(outputPath, directory);
        
        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);
        
        var i = 0;

        foreach (HtmlNode link in collection)
        {
            i++;
            var src = link.GetAttributeValue(attribute, string.Empty);
                
            if (string.IsNullOrWhiteSpace(src))
                continue;
            
            if (!src.StartsWith("/"))
                continue;

            string targetUrl = url + src;

            var fileName = new Uri(targetUrl).AbsolutePath.Split('/').Last();
            var isFile = fileName.Contains('.');
            if (!isFile)
                continue;

            var rawData = await GetBytesByUrl(targetUrl);
            if (!rawData.Any())
                continue;

            var fileNameTemplate = $"{i}_{fileName}";
            string path = Path.Combine(targetPath, fileNameTemplate);
            await File.WriteAllBytesAsync(path, rawData);
            var attributePath = Path.Combine(directory, fileNameTemplate).Replace("\\", "/");
            link.SetAttributeValue(attribute, $"./{attributePath}");
        }
    }
}
