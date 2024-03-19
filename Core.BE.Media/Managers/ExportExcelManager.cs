using Emeint.Core.BE.Media.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Managers
{
    public class ExportExcelManager : IExportExcelManager
    {
        #region Fields
        private IHostingEnvironment _hostingEnvironment;
        private IMediaStorageManager _mediaStorageManager;
        private IConfiguration _configuration;
        #endregion

        #region Constructors
        public ExportExcelManager(IHostingEnvironment hostingEnvironment, IMediaStorageManager mediaStorageManager, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _mediaStorageManager = mediaStorageManager;
            _configuration = configuration;
        }

        #endregion

        #region Methods

        public async Task<string> GenerateExcel<T>(string fileNamePrefix, List<T> dataList, string sheetName, string configuredFolderPath, string microserviceUrlPart, string dateFormat, Dictionary<string, string> columnsDictionary)
        {
            string path = DocPathUtility.CreateFolder(_hostingEnvironment.WebRootPath + configuredFolderPath);
            fileNamePrefix = string.IsNullOrEmpty(fileNamePrefix) ? "Data" : fileNamePrefix;
            string fileName = $"{fileNamePrefix}_{DateTime.UtcNow:yyyyMMddHHmmssffff}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(path, fileName));

            using (ExcelPackage package = new ExcelPackage(file))
            {
                //Add new worksheet
                sheetName = string.IsNullOrEmpty(sheetName) ? "Data" : sheetName;
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

                //Getting number of rows and columns
                int totalRows = dataList.Count;
                int totalColumns = columnsDictionary?.Count ?? typeof(T).GetProperties().Length;

                //Filling header row with property names
                for (int i = 1; i <= totalColumns; i++)
                {
                    //Return complex property name with separated words
                    string pascalToSeparated = (columnsDictionary == null || columnsDictionary.Count == 0)
                        ? Regex.Replace(Regex.Replace(typeof(T).GetProperties()[i - 1].Name, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2")
                        : columnsDictionary[columnsDictionary.Keys.ElementAt(i - 1)];

                    worksheet.Cells[1, i].Value = pascalToSeparated;
                    worksheet.Cells[1, i].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                }

                //Filling data from list
                for (int row = 2; row <= totalRows + 1; row++)
                {
                    for (int col = 1; col <= totalColumns; col++)
                    {
                        var propValue = (columnsDictionary == null || columnsDictionary.Count == 0)
                            ? typeof(T).GetProperties()[col - 1].GetValue(dataList[row - 2])
                            : typeof(T).GetProperty(columnsDictionary.Keys.ElementAt(col - 1))?.GetValue(dataList[row - 2]);

                        worksheet.Cells[row, col].Value = propValue;
                        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                }

                //change date cells formatting 
                //"dd/MM/yyyy hh:mm"
                UpdateDateCellsFormat<T>(totalRows, worksheet, dateFormat);

                //Set style for header row
                using (var range = worksheet.Cells[1, 1, 1, totalColumns])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Font.Color.SetColor(Color.White);

                }

                //Set style for the whole sheet
                using (var range = worksheet.Cells[1, 1, totalRows + 1, totalColumns])
                {
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.AutoFitColumns(10, 1000);
                    worksheet.View.FreezePanes(2, 1);
                }

                //Save excel file on the server 
                package.Save();
            }
            string filePath = $"/{microserviceUrlPart}{configuredFolderPath}{file.Name}";

            if (_configuration["SaveExportedExcelFileToAzureStorage"] == true.ToString())
            {
                filePath = await _mediaStorageManager.UploadExcelDocumentAsync(file, null);

                if (File.Exists(file.FullName))
                    File.Delete(file.FullName);
            }

            return filePath;
        }

        public void UpdateDateCellsFormat<T>(int rowsCount, ExcelWorksheet worksheet, string dateFormat)
        {
            var properties = typeof(T).GetProperties();
            properties.Select((p, i) => new { property = p, index = i })
                .Where(e => e.property.PropertyType.Name == "DateTime" || Nullable.GetUnderlyingType(e.property.PropertyType)?.Name == "DateTime")
                .ToList()
                .ForEach(e => worksheet.Cells[2, e.index + 2, rowsCount + 1, e.index + 2].Style.Numberformat.Format = dateFormat);
        }
        public void SetExcelStyle()
        {

        }
        #endregion
    }
}
