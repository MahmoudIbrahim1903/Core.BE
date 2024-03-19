using CsvHelper;
using Emeint.Core.BE.Media.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Managers
{
    public class ExportCsvManager : IExportCsvManager
    {
        #region Fields
        private IHostingEnvironment _hostingEnvironment;
        private IMediaStorageManager _mediaStorageManager;
        private IConfiguration _configuration;
        private readonly ILogger<ExportCsvManager> _logger;
        #endregion

        #region Constructors
        public ExportCsvManager(IHostingEnvironment hostingEnvironment, IMediaStorageManager mediaStorageManager, IConfiguration configuration, ILogger<ExportCsvManager> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _mediaStorageManager = mediaStorageManager;
            _configuration = configuration;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async Task<string> GenerateCsv<T>(string fileNamePrefix, List<T> dataList, string configuredFolderPath, string microserviceUrlPart, Dictionary<string, string> columnsDictionary)
        {
            string path = DocPathUtility.CreateFolder(_hostingEnvironment.WebRootPath + configuredFolderPath);
            fileNamePrefix = string.IsNullOrEmpty(fileNamePrefix) ? "Data" : fileNamePrefix;
            string fileName = $"{fileNamePrefix}_{DateTime.UtcNow:yyyyMMddHHmmssffff}.csv";
            FileInfo file = new FileInfo(Path.Combine(path, fileName));

            //Getting number of rows and columns
            int totalRows = dataList.Count;
            int totalColumns = columnsDictionary?.Count ?? typeof(T).GetProperties().Length;

            _logger.LogInformation($"track generating files. start generating Csv file. total rows = {totalRows}, total Columns = {totalColumns}, full Name = {file.FullName}");
            using (StreamWriter streamWriter = new StreamWriter(file.FullName, false, new UTF8Encoding(true)))
            {
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    //Filling header row with property names
                    for (int i = 1; i <= totalColumns; i++)
                    {
                        string pascalToSeparated = (columnsDictionary == null || columnsDictionary.Count == 0)
                            ? Regex.Replace(Regex.Replace(typeof(T).GetProperties()[i - 1].Name, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2")
                            : columnsDictionary[columnsDictionary.Keys.ElementAt(i - 1)];

                        csvWriter.WriteField(pascalToSeparated);
                    }
                    csvWriter.NextRecord();

                    //Filling data from list into rows 
                    for (int row = 2; row <= totalRows + 1; row++)
                    {
                        for (int col = 1; col <= totalColumns; col++)
                        {
                            var propValue = (columnsDictionary == null || columnsDictionary.Count == 0)
                                ? typeof(T).GetProperties()[col - 1].GetValue(dataList[row - 2])
                                : typeof(T).GetProperty(columnsDictionary.Keys.ElementAt(col - 1))?.GetValue(dataList[row - 2]);

                            csvWriter.WriteField(propValue);
                        }
                        //_logger.LogInformation($"track generating files. row {row}");
                        csvWriter.NextRecord();
                    }
                }
            }

            if (_configuration["SaveExportedExcelFileToAzureStorage"] == true.ToString())
            {
                _logger.LogInformation($"track generating files. before uploading Csv file");
                var url = await _mediaStorageManager.UploadExcelDocumentAsync(file, "text/csv");
                _logger.LogInformation($"track generating files. after uploading Csv file. url = {url}");

                // remove local file after uploaded to azure
                if (File.Exists(file.FullName))
                    File.Delete(file.FullName);

                return url;
            }

            return $"/{microserviceUrlPart}{configuredFolderPath}{file.Name}";
        }

        #endregion
    }
}
