namespace PublicProject.Factories
{
    using System.Collections.Generic;
    using System.Linq;

    using _Interfaces_;
    using _Interfaces_.Factories;

    using FileSystemProject;

    using Helper;

    using SdkProject.Api.Sync;
    using SdkProject.Api.Sync.Common;

    public class SyncStateFilesResponseFactory : ISyncStateFilesResponseFactory
    {
        private readonly IEnumerable<IFilesComparing> _fileComparings;
        private readonly IFileInfoModelFactory _fileInfoModelFactory;

        public SyncStateFilesResponseFactory(IEnumerable<IFilesComparing> fileComparings, IFileInfoModelFactory fileInfoModelFactory)
        {
            _fileComparings = fileComparings;
            _fileInfoModelFactory = fileInfoModelFactory;
        }

        public SyncStateFilesResponse Build(IList<string> databaseFiles, IList<FolderItem> deviceFolders, IList<DictionaryModel> serverDictionaries)
        {
            var response = new SyncStateFilesResponse();

            List<DictionaryModel> deviceFiles = deviceFolders.Select(Convert).ToList();

            foreach (DictionaryModel deviceFolder in deviceFiles)
            {
                foreach (IFilesComparing filesComparing in _fileComparings.ToList())
                {
                    List<string> dataBaseFiles = databaseFiles.Where(x => x.StartsWith(deviceFolder.Path)).ToList();
                    DictionaryModel serverFiles = serverDictionaries.FirstOrDefault(x => x.Path == deviceFolder.Path);

                    filesComparing.Apply(response, deviceFolder.Files, dataBaseFiles, serverFiles?.Files);
                }
            }

            return response;
        }

        private DictionaryModel Convert(FolderItem folderItem)
        {
            var dictionaryModel = new DictionaryModel(PathHelper.GetRawPath(folderItem.DictionaryPath));
            foreach (FileItem fileItem in folderItem.Files)
            {
                string rawPath = PathHelper.GetRawPath(fileItem.Path);
                FileInfoModel model = _fileInfoModelFactory.Create(rawPath, fileItem.Size, fileItem.Timestamp);
                dictionaryModel.Files.Add(model);
            }

            return dictionaryModel;
        }
    }
}
