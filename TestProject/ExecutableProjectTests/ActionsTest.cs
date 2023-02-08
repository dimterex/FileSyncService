using System;
using System.Collections.Generic;
using System.Linq;
using FileSystemProject;
using NSubstitute;
using NUnit.Framework;
using PublicProject.Logic.Comparing;
using SdkProject.Api.Sync;

namespace TestProject.ExecutableProjectTests
{
    [TestFixture]
    public class ActionsTest
    {
        private const string Remove_it_on_server_side = "Remove it on server side";
        private const string Remove_it_on_client_side = "Remove it on client side";
        private const string Update_it_on_client_side = "Update it on client side";

        private const string Add_it_on_server_side = "Add it on server side";
        private const string Add_it_on_client_side = "Add it on client side";

        private const long LAST_MODIFIED_TIMESTAMP = 1; 

        [Test]
        public void ActionTest()
        {
            // Arrange
            var databaseFiles = new List<string>
            {
                Remove_it_on_server_side,
                Remove_it_on_client_side,
                Update_it_on_client_side
            };

            var deviceFiles = GetClientDictionaryModels();
            var serverFiles = GetServerDictionaryModels();
            var fileManager = Substitute.For<IFileManager>();
            fileManager
                .TryGetFileInfo(Arg.Any<string>(), out Arg.Any<FileInfoModel>())
                .Returns(x => { 
                    x[1] = GetInfoModel(string.Empty, 0, 0);
                    return true;
                });

            // Act
            var serverRemoveFiles = new ServerRemoveFiles();
            var response = new SyncStateFilesResponse();
            serverRemoveFiles.Apply(response, deviceFiles[0].Files, databaseFiles, serverFiles[0].Files);
            Assert.AreEqual(response.ServerRemovedFiles.Count, 1);
            Assert.AreEqual(response.ServerRemovedFiles.FirstOrDefault()?.FileName, new[] { Remove_it_on_server_side });

            // Act
            var clientRemoveFiles = new ClientRemoveFiles();
            clientRemoveFiles.Apply(response, deviceFiles[0].Files, databaseFiles, serverFiles[0].Files);
            Assert.AreEqual(response.RemovedFiles.Count, 1);
            Assert.AreEqual(response.RemovedFiles.FirstOrDefault()?.FileName, new[] { Remove_it_on_client_side });

            // Act
            var serverAddFiles = new ServerAddFiles(fileManager);
            serverAddFiles.Apply(response, deviceFiles[0].Files, databaseFiles, serverFiles[0].Files);
            Assert.AreEqual(response.UploadedFiles.Count, 1);
            Assert.AreEqual(response.UploadedFiles.FirstOrDefault()?.FileName, new[] { Add_it_on_server_side });

            // Act
            var clientAddFiles = new ClientAddFiles();
            clientAddFiles.Apply(response, deviceFiles[0].Files, databaseFiles, serverFiles[0].Files);
            Assert.AreEqual(response.AddedFiles.Count, 1);
            Assert.AreEqual(response.AddedFiles.FirstOrDefault()?.FileName, new[] { Add_it_on_client_side });

            // Act
            var clientUpdateFiles = new ClientUpdateFiles();
            clientUpdateFiles.Apply(response, deviceFiles[0].Files, databaseFiles, serverFiles[0].Files);
            Assert.AreEqual(response.UpdatedFiles.Count, 1);
            Assert.AreEqual(response.UpdatedFiles.FirstOrDefault()?.FileName, new[] { Update_it_on_client_side });
        }

        private FileInfoModel GetInfoModel(string path, long size, long lastWriteTimeUtc)
        {
            return new FileInfoModel(path, size, new DateTime(lastWriteTimeUtc));
        }

        private IList<DictionaryModel> GetClientDictionaryModels()
        {
            var deviceFiles = new List<DictionaryModel>();
            var dictionaryModel = new DictionaryModel(string.Empty);
            dictionaryModel.Files.Add(GetInfoModel(Remove_it_on_client_side, 1, LAST_MODIFIED_TIMESTAMP));
            dictionaryModel.Files.Add(GetInfoModel(Add_it_on_server_side, 1, LAST_MODIFIED_TIMESTAMP));
            dictionaryModel.Files.Add(GetInfoModel(Update_it_on_client_side, 1, LAST_MODIFIED_TIMESTAMP));
            deviceFiles.Add(dictionaryModel);
            return deviceFiles;
        }

        private IList<DictionaryModel> GetServerDictionaryModels()
        {
            var deviceFiles = new List<DictionaryModel>();
            var dictionaryModel = new DictionaryModel(string.Empty);
            dictionaryModel.Files.Add(GetInfoModel(Remove_it_on_server_side, 1, LAST_MODIFIED_TIMESTAMP));
            dictionaryModel.Files.Add(GetInfoModel(Add_it_on_client_side, 1, LAST_MODIFIED_TIMESTAMP));
            dictionaryModel.Files.Add(GetInfoModel(Update_it_on_client_side, 2, LAST_MODIFIED_TIMESTAMP));
            deviceFiles.Add(dictionaryModel);
            return deviceFiles;
        }
    }
}