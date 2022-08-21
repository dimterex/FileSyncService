using System.Collections.Generic;
using System.IO;
using FileSystemProject;
using NSubstitute;
using NUnit.Framework;
using PublicProject._Interfaces_;
using PublicProject.Factories;
using SdkProject.Api.Sync;
using SdkProject.Api.Sync.Common;

namespace TestProject.ExecutableProjectTests
{
    [TestFixture]
    public class SyncStateFilesResponseFactoryTest
    {
        private const string DICTIONARY_PATH = "Dictionary Path";
        private const string SECOND_DICTIONARY_PATH = "Second dictionary Path";

        private const string FIRST_FILE_PATH = "File Path";
        private const string SECOND_FILE_PATH = "Second file Path";

        [Test]
        public void BuildTest()
        {
            var testComparing = Substitute.For<IFilesComparing>();

            testComparing.When(x => x.Apply(Arg.Any<SyncStateFilesResponse>(),
                Arg.Any<List<FileInfoModel>>(),
                Arg.Any<List<string>>(),
                Arg.Any<List<FileInfoModel>>())).Do(x =>
            {
                var response = x[0] as SyncStateFilesResponse;
                var deviceFolderFiles = x[1] as IList<FileInfoModel>;
                var filesFromDataBase = x[2] as IList<string>;
                var filesFromServer = x[3] as IList<FileInfoModel>;

                Assert.AreEqual(deviceFolderFiles.Count, 1);
                Assert.AreEqual(filesFromDataBase.Count, 2);
                Assert.AreEqual(filesFromServer.Count, 2);
            });
            var syncStateFilesResponseFactory = new SyncStateFilesResponseFactory(new[] { testComparing });

            var databaseFiles = new List<string>
            {
                Path.Combine(DICTIONARY_PATH, FIRST_FILE_PATH),
                Path.Combine(DICTIONARY_PATH, SECOND_FILE_PATH),
                Path.Combine(SECOND_DICTIONARY_PATH, SECOND_FILE_PATH),
                Path.Combine(SECOND_DICTIONARY_PATH, SECOND_FILE_PATH)
            };

            var firstDeviceFolderItem = new FolderItem();
            firstDeviceFolderItem.DictionaryPath = DICTIONARY_PATH;
            firstDeviceFolderItem.Files.Add(new FileItem { Path = new[] { DICTIONARY_PATH, FIRST_FILE_PATH } });

            var deviceFolders = new List<FolderItem>
            {
                firstDeviceFolderItem
            };

            var firstServerDictionary = new DictionaryModel(DICTIONARY_PATH);
            firstServerDictionary.Files.Add(new FileInfoModel(Path.Combine(DICTIONARY_PATH, FIRST_FILE_PATH), 1));
            firstServerDictionary.Files.Add(new FileInfoModel(Path.Combine(DICTIONARY_PATH, SECOND_FILE_PATH), 1));

            var secondServerDictionary = new DictionaryModel(SECOND_DICTIONARY_PATH);
            secondServerDictionary.Files.Add(
                new FileInfoModel(Path.Combine(SECOND_DICTIONARY_PATH, FIRST_FILE_PATH), 1));
            secondServerDictionary.Files.Add(new FileInfoModel(Path.Combine(SECOND_DICTIONARY_PATH, SECOND_FILE_PATH),
                1));

            var serverDictionaries = new List<DictionaryModel>
            {
                firstServerDictionary, secondServerDictionary
            };

            syncStateFilesResponseFactory.Build(databaseFiles, deviceFolders, serverDictionaries);
        }
    }
}