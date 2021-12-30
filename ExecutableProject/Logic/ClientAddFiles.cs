using System.Collections.Generic;
using System.Linq;
using FileSystemProject;

namespace ExecutableProject.Logic
{
    public class ClientAddFiles
    {
        public IList<FileInfoModel> Get(IList<string> filesFromDataBase, List<FileInfoModel> filesFromDevice, List<FileInfoModel> filesFromServer)
        {
            var result = new List<FileInfoModel>();

            foreach (var fileFromServer in filesFromServer)
            {
                var databasePath = filesFromDataBase.FirstOrDefault(x => x == fileFromServer.Path);
                if (databasePath != null)
                {
                    continue;
                }
                
                var devicePath = filesFromDevice.FirstOrDefault(x => x.Path == fileFromServer.Path);
                if (devicePath != null)
                {
                    continue;
                }
                result.Add(fileFromServer);
            }
            
            return result;
        }
    }
}