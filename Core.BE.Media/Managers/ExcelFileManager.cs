using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Emeint.Core.BE.Media.Managers
{
    public class ExcelFileManager : IExcelFileManager
    {
        public bool IsValidColumnsHeader(SheetData sheetData, WorkbookPart workbookPart, List<(int index, string name)> columnsInfo)
        {
            foreach (var columnInfo in columnsInfo)
            {
                (string cellValue, bool isValidFormat, bool isEmpty) = GetCellResult<string>(sheetData, workbookPart, 0, columnInfo.index);
                if (cellValue == null || cellValue.ToLower() != columnInfo.name.ToLower())
                    return false;
            }
            return true;
        }

        public (T value, bool isValidFormat, bool isEmpty) GetCellResult<T>(SheetData sheetData, WorkbookPart workbookPart, int rowNumber, int columnNumber)
        {
            string cellName = GetColumnName(columnNumber + 1) + (rowNumber + 1).ToString();
            Cell cell = sheetData.Descendants<Cell>().Where(c => c.CellReference == cellName).FirstOrDefault();

            if (cell == null)
            {
                //if (sheetData.ElementAt(0).ChildElements.Count() - 1 >= columnNumber)
                return (default(T), true, true);
            }

            switch (typeof(T))
            {
                case Type intType when intType == typeof(int):
                    if (string.IsNullOrEmpty(cell.InnerText))
                        return (default(T), true, true);
                    else if ((cell.DataType != null && cell.DataType == CellValues.SharedString) || !int.TryParse(cell.InnerText, out int intOutPut))
                        return (default(T), false, false);
                    else
                        return ((T)(object)intOutPut, true, false);
                case Type intType when intType == typeof(int?):
                    if (string.IsNullOrEmpty(cell.InnerText))
                        return ((T)(object)null, true, true);
                    else if ((cell.DataType != null && cell.DataType == CellValues.SharedString) || !int.TryParse(cell.InnerText, out int nullableIntOutPut))
                        return (default(T), false, false);
                    else
                        return ((T)(object)nullableIntOutPut, true, false);
                case Type stringType when stringType == typeof(string):
                    if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                    {
                        int id;
                        if (Int32.TryParse(cell.InnerText, out id))
                        {
                            SharedStringItem item = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                            if (string.IsNullOrEmpty(item.Text.Text))
                                return ((T)(object)item.Text.Text, true, true);
                            else
                                return ((T)(object)item.Text.Text, true, false);
                        }
                        else
                            return (default(T), false, false);
                    }
                    else if (string.IsNullOrEmpty(cell.InnerText))
                        return ((T)(object)null, true, true);
                    else
                        return ((T)(object)cell.InnerText, true, false);
                case Type decimalType when decimalType == typeof(decimal):
                    if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                        return (default(T), false, false);
                    else if (string.IsNullOrEmpty(cell.InnerText))
                        return (default(T), true, true);
                    else
                        return ((T)(object)Convert.ToDecimal(cell.InnerText), true, false);
                case Type decimalType when decimalType == typeof(decimal?):
                    if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                        return (default(T), false, false);
                    else if (string.IsNullOrEmpty(cell.InnerText))
                        return ((T)(object)null, true, true);
                    else
                        return ((T)(object)Convert.ToDecimal(cell.InnerText), true, false);
                case Type booleanType when booleanType == typeof(bool?):
                    if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                    {
                        int id;
                        if (Int32.TryParse(cell.InnerText, out id))
                        {
                            SharedStringItem item = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                            if (string.IsNullOrEmpty(item.Text.Text))
                                return ((T)(object)item.Text.Text, true, true);
                            else
                            {
                                if (item.Text.Text.ToLower() == "yes" || item.Text.Text.ToLower() == "true")
                                    return ((T)(object)true, true, false);
                                else if (item.Text.Text.ToLower() == "no" || item.Text.Text.ToLower() == "false")
                                    return ((T)(object)false, true, false);
                                else
                                    return (default(T), false, false);
                            }
                        }
                        else
                            return (default(T), false, false);
                    }
                    else if (string.IsNullOrEmpty(cell.InnerText))
                        return ((T)(object)null, true, true);
                    else
                        return (default(T), false, false);
                default:
                    return (default(T), false, false);
            }
        }

        public string GetCellFullName(int rowNumber, int columnNumber)
        {
            string cellFullName = GetColumnName(columnNumber + 1) + (rowNumber + 1).ToString();
            return cellFullName;
        }

        public bool IsEmptyRow(SheetData sheetData, int rowNumber, int columnsCount)
        {
            for (int columnNumber = 0; columnNumber < columnsCount; columnNumber++)
            {
                string cellName = GetColumnName(columnNumber + 1) + (rowNumber + 1).ToString();
                Cell cell = sheetData.Descendants<Cell>().Where(c => c.CellReference == cellName).FirstOrDefault();

                if (cell != null && cell.CellValue != null && (columnsCount > columnNumber && !string.IsNullOrEmpty(cell.CellValue.ToString())))
                    return false;
            }
            return true;
        }

        private string GetColumnName(int columnNumber)
        {
            string name = "";
            while (columnNumber > 0)
            {
                columnNumber--;
                name = (char)('A' + columnNumber % 26) + name;
                columnNumber /= 26;
            }
            return name;
        }

        public int GetLastValidRowNumber(SheetData sheetData)
        {
            var lastValidCell = sheetData.Descendants<Cell>().LastOrDefault(c => c != null && c.CellValue != null && !string.IsNullOrWhiteSpace(c.CellValue.ToString()));

            if (lastValidCell.CellValue == null)
                return 0;

            Regex regex = new Regex("[A-Za-z]+");
            var lastCellName = lastValidCell.CellReference.ToString();
            string columnName = regex.Match(lastCellName).Value;
            int rowNumber = Convert.ToInt32(lastCellName.Replace(columnName, String.Empty));

            return rowNumber;
        }
    }
}
