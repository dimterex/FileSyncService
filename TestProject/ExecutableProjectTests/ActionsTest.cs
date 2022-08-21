using System.Collections.Generic;
using System.Linq;
using FileSystemProject;
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

        [Test]
        public void ActionTest()
        {
            var databaseFiles = new List<string>
            {
                Remove_it_on_server_side,
                Remove_it_on_client_side,
                Update_it_on_client_side
            };

            var deviceFiles = GetClientDictionaryModels();
            var serverFiles = GetServerDictionaryModels();

            var serverRemoveFiles = new ServerRemoveFiles();
            var response = new SyncStateFilesResponse();
            serverRemoveFiles.Apply(response, deviceFiles[0].Files, databaseFiles, serverFiles[0].Files);
            Assert.AreEqual(response.ServerRemovedFiles.Count, 1);
            Assert.AreEqual(response.ServerRemovedFiles.FirstOrDefault()?.FileName, new[] { Remove_it_on_server_side });

            var clientRemoveFiles = new ClientRemoveFiles();
            clientRemoveFiles.Apply(response, deviceFiles[0].Files, databaseFiles, serverFiles[0].Files);
            Assert.AreEqual(response.RemovedFiles.Count, 1);
            Assert.AreEqual(response.RemovedFiles.FirstOrDefault()?.FileName, new[] { Remove_it_on_client_side });

            var serverAddFiles = new ServerAddFiles();
            serverAddFiles.Apply(response, deviceFiles[0].Files, databaseFiles, serverFiles[0].Files);
            Assert.AreEqual(response.UploadedFiles.Count, 1);
            Assert.AreEqual(response.UploadedFiles.FirstOrDefault()?.FileName, new[] { Add_it_on_server_side });

            var clientAddFiles = new ClientAddFiles();
            clientAddFiles.Apply(response, deviceFiles[0].Files, databaseFiles, serverFiles[0].Files);
            Assert.AreEqual(response.AddedFiles.Count, 1);
            Assert.AreEqual(response.AddedFiles.FirstOrDefault()?.FileName, new[] { Add_it_on_client_side });

            var clientUpdateFiles = new ClientUpdateFiles();
            clientUpdateFiles.Apply(response, deviceFiles[0].Files, databaseFiles, serverFiles[0].Files);
            Assert.AreEqual(response.UpdatedFiles.Count, 1);
            Assert.AreEqual(response.UpdatedFiles.FirstOrDefault()?.FileName, new[] { Update_it_on_client_side });
        }

        private IList<DictionaryModel> GetClientDictionaryModels()
        {
            var deviceFiles = new List<DictionaryModel>();
            var dictionaryModel = new DictionaryModel(string.Empty);
            dictionaryModel.Files.Add(new FileInfoModel(Remove_it_on_client_side, 1));
            dictionaryModel.Files.Add(new FileInfoModel(Add_it_on_server_side, 1));
            dictionaryModel.Files.Add(new FileInfoModel(Update_it_on_client_side, 1));
            deviceFiles.Add(dictionaryModel);
            return deviceFiles;
        }

        private IList<DictionaryModel> GetServerDictionaryModels()
        {
            var deviceFiles = new List<DictionaryModel>();
            var dictionaryModel = new DictionaryModel(string.Empty);
            dictionaryModel.Files.Add(new FileInfoModel(Remove_it_on_server_side, 1));
            dictionaryModel.Files.Add(new FileInfoModel(Add_it_on_client_side, 1));
            dictionaryModel.Files.Add(new FileInfoModel(Update_it_on_client_side, 2));
            deviceFiles.Add(dictionaryModel);
            return deviceFiles;
        }
    }
}