using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Managers
{
    public interface IExportCsvManager
    {
        Task<string> GenerateCsv<T>(string fileNamePrefix, List<T> dataList, string configuredFolderPath, string microserviceUrlPart, Dictionary<string, string> columnsDictionary = null);
    }
}
