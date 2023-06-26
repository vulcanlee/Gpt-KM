using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;

namespace GptLibrary.Converts
{
    public class ExcelToText : IFileToText
    {
        public Task<string> ToTextAsync(string filePath)
        {
            var task = Task.Run(() =>
            {
                StringBuilder sb = new StringBuilder();

                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault();
                    if (sheet != null)
                    {
                        WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                        SharedStringTablePart sharedStringTablePart = workbookPart.SharedStringTablePart;
                        var rows = worksheetPart.Worksheet.Descendants<Row>();

                        foreach (var row in rows)
                        {
                            var cells = row.Elements<Cell>();
                            bool firstCell = true;

                            foreach (var cell in cells)
                            {
                                if (!firstCell)
                                {
                                    sb.Append(",");
                                }

                                string cellValue = GetCellValue(cell, sharedStringTablePart);
                                sb.Append(QuoteValueIfNeeded(cellValue));

                                firstCell = false;
                            }

                            sb.AppendLine();
                        }
                    }
                }

                return sb.ToString();
            });
            return task;
        }

        string GetCellValue(Cell cell, SharedStringTablePart sharedStringTablePart)
        {
            if (cell == null || cell.CellValue == null)
            {
                return "";
            }

            string value = cell.CellValue.InnerText;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                value = sharedStringTablePart.SharedStringTable.ChildElements[int.Parse(value)].InnerText;
            }

            return value;
        }

        string QuoteValueIfNeeded(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            else
            {
                return value;
            }
        }
    }
}