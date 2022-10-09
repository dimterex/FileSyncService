using System.Collections.Generic;
using System.Linq;
using FileSystemProject;
using PublicProject._Interfaces_;
using PublicProject._Interfaces_.Factories;
using PublicProject.Helper;
using SdkProject.Api.Sync;
using SdkProject.Api.Sync.Common;

namespace PublicProject.Factories
{
    public class SyncStateFilesResponseFactory : ISyncStateFilesResponseFactory
    {
        private readonly IEnumerable<IFilesComparing> _fileComparings;

        public SyncStateFilesResponseFactory(IEnumerable<IFilesComparing> fileComparings)
        {
            _fileComparings = fileComparings;
        }

        public SyncStateFilesResponse Build(IList<string> databaseFiles, IList<FolderItem> deviceFolders,
            IList<DictionaryModel> serverDictionaries)
        {
            var response = new SyncStateFilesResponse();

            var deviceFiles = deviceFolders.Select(Convert).ToList();

            foreach (var deviceFolder in deviceFiles)
            foreach (var filesComparing in _fileComparings.ToList())
            {
                var dataBaseFiles = databaseFiles.Where(x => x.StartsWith(deviceFolder.Path)).ToList();
                var serverFiles = serverDictionaries.FirstOrDefault(x => x.Path == deviceFolder.Path);

                filesComparing.Apply(response, deviceFolder.Files, dataBaseFiles, serverFiles?.Files);
            }

            return response;
        }

        private DictionaryModel Convert(FolderItem folderItem)
        {
            var dictionaryModel = new DictionaryModel(PathHelper.GetRawPath(folderItem.DictionaryPath));
            foreach (var fileItem in folderItem.Files)
            {
                var rawPath = PathHelper.GetRawPath(fileItem.Path);
                dictionaryModel.Files.Add(new FileInfoModel(rawPath, fileItem.Size));
            }

            return dictionaryModel;
        }
    }
}