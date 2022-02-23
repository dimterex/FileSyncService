using System.Collections.Generic;
using System.Linq;
using FileSystemProject;

namespace PublicProject.Logic
{
    public class ClientUpdateFiles
    {
        public IList<FileInfoModel> Get(IList<string> filesFromDataBase, List<FileInfoModel> filesFromDevice, List<FileInfoModel> filesFromServer)
        {
            var result = new List<FileInfoModel>();

            foreach (var fileFromDevice in filesFromDevice)
            {
                var databasePath = filesFromDataBase.FirstOrDefault(x => x == fileFromDevice.Path);
                if (databasePath == null)
                {
                    continue;
                }
                
                var serverPath = filesFromServer.FirstOrDefault(x => x.Path == fileFromDevice.Path);
                if (serverPath == null)
                {
                    continue;
                }
                
                if (serverPath.Size != fileFromDevice.Size)
                {
                    result.Add(fileFromDevice);
                }
            }
            
            return result;
        }
    }
}