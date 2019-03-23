using Service.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Service.Api.Message;

namespace Service.Api
{
    public class JsonManager
    {
        // private const string ROOT_PATH = "E:\\Синхронизация\\Музыка";
        private const int BUFFER_SIZE = 1024;
        private const string ROOT_PATH = "E:\\Downloads";

        private FileManager _fileManager;

        public JsonManager(FileManager fileManager)
        {
            _fileManager = fileManager;
            //SyncFilesResponceExecute(new SyncFilesRequest(), null);
        }

        public void Execute(string data, TcpClient client)
        {
            JMessage message = JMessage.Deserialize(data);

            switch (message.Type)
            {
                #region На стороне клиента

                case nameof(FileAddResponce):
                    break;
                case nameof(FileRemoveResponce):
                    break;
                case nameof(FileListResponce):
                    break;

                case nameof(SaveFileResponce):
                    break;

                #endregion

                case nameof(SaveFileRequest):
                    SaveFileRequest saveFileRequest = message.Value.ToObject<SaveFileRequest>();
                    break;

                case nameof(FileListRequest):
                    FileListRequestExecute(client);
                    break;
                case nameof(SyncFilesRequest):
                    SyncFilesRequest syncFilesRequest = message.Value.ToObject<SyncFilesRequest>();
                    SyncFilesResponceExecute(syncFilesRequest, client);
                    break;
                default:
                    Console.WriteLine($"{message.Type} не поддерживается.");
                    return;
            }
        }

        private void FileListRequestExecute(TcpClient client)
        {
            var result = new FileListResponce();
            var tmp = _fileManager.GetFileList(ROOT_PATH);
            result.Files.AddRange(tmp);
            SendData(result, client.GetStream());
            Console.WriteLine("FileListResponce Done!");
        }

        private void SyncFilesResponceExecute(SyncFilesRequest fileAction, TcpClient client)
        {
            var root_files = _fileManager.GetFileList(ROOT_PATH);
            var not_exist_in_server = new List<BaseFileInfo>();
            var not_exist_in_client = new List<BaseFileInfo>();
            _fileManager.CompairFolders(not_exist_in_server, fileAction.Files, root_files, ROOT_PATH);
            _fileManager.CompairFolders(not_exist_in_client, root_files, fileAction.Files, ROOT_PATH);

            SendFilesAddResponce(not_exist_in_client, client);
            SendFilesRemoveResponce(not_exist_in_server, client);
        }

        private void SendFilesAddResponce(List<BaseFileInfo> not_exist_in_client, TcpClient client)
        {
            foreach (BaseFileInfo baseFileInfo in not_exist_in_client)
            {
                var realPath = GetRealPath(baseFileInfo);

                FileInfo fileInfo = new FileInfo(realPath);
                byte[] buff = File.ReadAllBytes(realPath);
                string base64EncodedExternalAccount = Convert.ToBase64String(buff);
                var tt = new FileAddResponce();
                tt.FileName = fileInfo.Name;
                tt.FilePath.AddRange(RemoveRootPath(fileInfo));
                tt.Stream = base64EncodedExternalAccount;
                SendData(tt, client.GetStream());

                Console.WriteLine($"    {realPath} sended!");
            }
        }

        private void SendFilesRemoveResponce(List<BaseFileInfo> not_exist_in_server, TcpClient client)
        {
            foreach (BaseFileInfo baseFileInfo in not_exist_in_server)
            {
                var tt = new FileRemoveResponce();
                tt.FileName = baseFileInfo.FileName;
                tt.FilePath.AddRange(baseFileInfo.FilePath);

                SendData(tt, client.GetStream());
                var fullPath = Path.Combine(baseFileInfo.FilePath.ToArray());
                Console.WriteLine($"    {Path.Combine(fullPath, baseFileInfo.FileName)} removed!");
            }
        }

        private string GetRealPath(BaseFileInfo baseFileInfo)
        {
            var path = Path.Combine(baseFileInfo.FilePath.ToArray());
            var filePath = Path.Combine(path, baseFileInfo.FileName);
            var realPath = Path.Combine(ROOT_PATH, filePath);
            return realPath;
        }

        private List<string> RemoveRootPath(FileInfo fileInfo)
        {
            List<string> directory = fileInfo.Directory.FullName.Split(Path.DirectorySeparatorChar).ToList();
            var path1 = ROOT_PATH.Split(Path.DirectorySeparatorChar).ToList();
            path1.ForEach(x => directory.Remove(x));
            return directory;
        }

        private void SendData<T>(T obj, NetworkStream stream)
        {
            string rawString = JMessage.Serialize(JMessage.FromValue(obj));
            byte[] data = Encoding.UTF8.GetBytes(rawString);

            byte[] dataLength = BitConverter.GetBytes(data.Length);
            stream.Write(dataLength, 0, 4);

            int bytesSent = 0;
            int bytesLeft = data.Length;

            while (bytesLeft > 0)
            {
                int curDataSize = Math.Min(BUFFER_SIZE, bytesLeft);
                stream.Write(data, bytesSent, curDataSize);
                bytesSent += curDataSize;
                bytesLeft -= curDataSize;
            }
        }
    }
}
