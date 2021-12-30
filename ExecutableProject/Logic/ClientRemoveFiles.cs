using System.Collections.Generic;
using System.Linq;
using FileSystemProject;

namespace ExecutableProject.Logic
{
    public class ClientRemoveFiles
    {
        public IList<FileInfoModel> Get(IList<string> filesFromDataBase, List<FileInfoModel> filesFromDevice, List<FileInfoModel> filesFromServer)
        {
            var result = new List<FileInfoModel>();

            foreach (var fileFromDataBase in filesFromDataBase)
            {
                foreach (var fileFromDevice in filesFromDevice)
                {
                    if (fileFromDevice.Path != fileFromDataBase)
                        continue;
                    
                    var devicePath = filesFromServer.FirstOrDefault(x => x.Path == fileFromDataBase);
                    if (devicePath == null)
                    {
                        result.Add(fileFromDevice);
                    }
                }
            }

            return result;
        }
    }
}