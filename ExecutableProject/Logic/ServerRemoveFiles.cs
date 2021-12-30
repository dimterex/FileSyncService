using System.Collections.Generic;
using System.Linq;
using FileSystemProject;
using SdkProject.Api.Sync;

namespace ExecutableProject.Logic
{
    public class ServerRemoveFiles
    {
        public ServerRemoveFiles()
        {
            
        }

        public IList<FileInfoModel> Get(IList<string> filesFromDataBase, List<FileInfoModel> filesFromDevice, List<FileInfoModel> filesFromServer)
        {
            var result = new List<FileInfoModel>();

            foreach (var fileFromDataBase in filesFromDataBase)
            {
                foreach (var fileFromServer in filesFromServer)
                {
                    if (fileFromServer.Path != fileFromDataBase)
                        continue;
                    
                    var devicePath = filesFromDevice.FirstOrDefault(x => x.Path == fileFromDataBase);
                    if (devicePath == null)
                    {
                        result.Add(fileFromServer);
                    }
                }
            }

            return result;
        }
    }
}