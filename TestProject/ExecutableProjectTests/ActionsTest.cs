using System.Collections.Generic;
using System.Linq;
using FileSystemProject;
using NUnit.Framework;
using PublicProject.Logic;

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
                Update_it_on_client_side,
                
            };
            
            var deviceFiles = new List<FileInfoModel>
            {
                new FileInfoModel(Remove_it_on_client_side, 1),
                new FileInfoModel(Add_it_on_server_side, 1),
                new FileInfoModel(Update_it_on_client_side, 1),
            };
            
            var serverFiles = new List<FileInfoModel>
            {
                new FileInfoModel(Remove_it_on_server_side, 1),
                new FileInfoModel(Add_it_on_client_side, 1),
                new FileInfoModel(Update_it_on_client_side, 2),
            };

            void CheckAssert(IList<FileInfoModel> result, string element)
            {
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result.FirstOrDefault()?.Path, element);
            }
            
            var serverRemoveFiles = new ServerRemoveFiles();
            var resultServerRemoveFiles = serverRemoveFiles.Get(databaseFiles, deviceFiles, serverFiles);
            CheckAssert(resultServerRemoveFiles, Remove_it_on_server_side);
            
            var clientRemoveFiles = new ClientRemoveFiles();
            var resultClientRemoveFiles = clientRemoveFiles.Get(databaseFiles, deviceFiles, serverFiles);
            CheckAssert(resultClientRemoveFiles, Remove_it_on_client_side);

            var serverAddFiles = new ServerAddFiles();
            var resultServerAddFiles = serverAddFiles.Get(databaseFiles, deviceFiles, serverFiles);
            CheckAssert(resultServerAddFiles, Add_it_on_server_side);

            var clientAddFiles = new ClientAddFiles();
            var resultClientAddFiles = clientAddFiles.Get(databaseFiles, deviceFiles, serverFiles);
            CheckAssert(resultClientAddFiles, Add_it_on_client_side);

            var clientUpdateFiles = new ClientUpdateFiles();
            var resultClientUpdateFiles = clientUpdateFiles.Get(databaseFiles, deviceFiles, serverFiles);
            CheckAssert(resultClientUpdateFiles, Update_it_on_client_side);
        }
    }
}