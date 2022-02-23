using System.Collections.Generic;
using System.Linq;
using FileSystemProject;

namespace PublicProject.Logic
{
    public class ClientServerExistFiles
    {
        public IList<FileInfoModel> Get(IList<string> filesFromDataBase, List<FileInfoModel> filesFromDevice, List<FileInfoModel> filesFromServer)
        {
            var result = new List<FileInfoModel>();

            foreach (var fileFromDevice in filesFromDevice)
            {
                var serverPath = filesFromServer.FirstOrDefault(x => x.Path == fileFromDevice.Path);
                if (serverPath == null)
                {
                    continue;
                }
                
                var databaseFile = filesFromDataBase.FirstOrDefault(x => x == fileFromDevice.Path);
                if (databaseFile != null)
                    continue;
                
                result.Add(fileFromDevice);
            }

          
            return result;
        }
    }
}