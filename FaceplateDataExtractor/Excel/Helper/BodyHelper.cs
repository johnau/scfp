using ClosedXML.Excel;
using System.Diagnostics;

namespace FaceplateDataExtractor.Excel.Helper
{
    internal class BodyHelper
    {
        public static void PopulateRowDatas(List<WorksheetRowData> rowDatas, IXLRows bodyRows, WorksheetHeaderData headerData)
        {
            // This is a weird place for this - should find somewhere else for it to live
            // Some values require fill for a consistent data set
            var defaultValues = new Dictionary<int, object>()
            {
                { headerData.RoomColumn, "<No ROOM, See: LOCATION>" }, // Room column
                { headerData.AboveFinishedFloorLevelColumn, 0 },
            };

            foreach (var row in bodyRows) { 
                var worksheetData = new WorksheetRowData(row.RowNumber());
                
                foreach (var cell in row.Cells())
                {
                    var colNumber = cell.WorksheetColumn().ColumnNumber();
                    var rowNumber = cell.WorksheetRow().RowNumber();
                    var header = headerData.GetHeadersByColumnIndex(colNumber);
                    if (cell.Value.IsText)
                    {
                        var text = cell.GetText().Trim();
                        worksheetData.AddRowData(header, text, colNumber, rowNumber);
                    }
                    else if (cell.Value.IsNumber)
                    {
                        worksheetData.AddRowData(header, cell.GetDouble(), colNumber, rowNumber);
                    }
                    else if (cell.Value.IsBlank)
                    {
                        if (defaultValues.TryGetValue(colNumber, out var defaultValue))
                            worksheetData.AddRowData(header, defaultValue + "", colNumber, rowNumber);
                        else
                            worksheetData.AddRowData(header, "", colNumber, rowNumber);
                    }
                    else
                    {
                        Debug.WriteLine($"Encountered an unsupported type: {cell.Value} ({cell.Value.Type})");
                        worksheetData.AddRowData(header, "<Unsupported Value!>", colNumber, rowNumber);
                    }
                }
                rowDatas.Add(worksheetData);
            }
        }

        /// <summary>
        /// Removes invalid rows from the data set
        /// </summary>
        /// <remarks>
        /// If <see cref="WorksheetHeaderData.PanelIdColumn">, <see cref="WorksheetHeaderData.DescriptionColumn"/>, or <see cref="WorksheetHeaderData.LocationColumn"/> does not have a value, the data row is discarded.
        /// </remarks>
        /// <param name="rowDatas"></param>
        /// <param name="headerData"></param>
        /// <returns>List of <see cref="WorksheetRowData"/> of discarded Rows</returns>
        public static List<WorksheetRowData> StripInvalidRows(List<WorksheetRowData> rowDatas, WorksheetHeaderData headerData)
        {
            // A row is discarded if the following columns do not have values
            var requiredColumns = new List<int>()
            {
                headerData.PanelIdColumn,
                headerData.DescriptionColumn,
                headerData.LocationColumn,
            };
            
            var invalidRows = new List<WorksheetRowData>();

            foreach (var rowData in rowDatas)
            {
                foreach (var dat in rowData.RowData)
                {
                    var cellData = dat.Value;
                    var cellValue = cellData.Value + "";
                    foreach (var requiredColNumber in requiredColumns)
                    {
                        if (cellData.ColumnNumber == requiredColNumber && cellValue == "")
                            invalidRows.Add(rowData);
                    }
                }
            }

            rowDatas.RemoveAll(invalidRows.Contains);

            return invalidRows;
        }
    }
}
