using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emeint.Core.BE.Media.Managers
{
    public interface IExcelFileManager
    {
        bool IsValidColumnsHeader(SheetData sheetData, WorkbookPart workbookPart, List<(int index, string name)> columnsInfo);
        (T value, bool isValidFormat, bool isEmpty) GetCellResult<T>(SheetData sheetData, WorkbookPart workbookPart, int rowNumber, int columnNumber);
        bool IsEmptyRow(SheetData sheetData, int rowNumber, int columnsCount);
        string GetCellFullName(int rowNumber, int columnNumber);
        int GetLastValidRowNumber(SheetData sheetData);
    }
}
