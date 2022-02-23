using System.Collections.Generic;
using System.Linq;
using FileSystemProject;

namespace PublicProject.Logic
{
    public class ClientRemoveFiles
    {
        public IList<FileInfoModel> Get(IList<string> filesFromDataBase, List<FileInfoModel> filesFromDevice, List<FileInfoModel> filesFromServer)
        {
            var result = new List<FileInfoModel>();

            foreach (var fileFromDataBase in filesFromDataBase)
            {
                var deviceFile = filesFromDevice.FirstOrDefault(x => x.Path == fileFromDataBase);
                if (deviceFile == null)
                    continue;
                
                var serverPath = filesFromServer.FirstOrDefault(x => x.Path == fileFromDataBase);
                if (serverPath == null)
                {
                    result.Add(deviceFile);
                }
            }

            return result;
        }
    }
}