using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Managers
{
    public interface IExportExcelManager
    {
        Task<string> GenerateExcel<T>(string fileNamePrefix, List<T> dataList, string sheetName, string configuredFolderPath, string microserviceUrlPart, string dateFormat = "dd/MM/yyyy hh:mm", Dictionary<string, string> columnsDictionary = null);
    }
}
