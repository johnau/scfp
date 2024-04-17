using ClosedXML.Excel;
using FaceplateDataExtractor.Model;
using System.Diagnostics;

namespace FaceplateDataExtractor.Excel.Helper
{
    internal class HeaderHelper
    {
        /// <summary>
        /// Scan each column top down through the header range and accumulate the header data
        /// </summary>
        public static void PopulateHeaderData(WorksheetHeaderData headers, IXLRows headerRows)
        {
            var insideMergedCell = false;
            var firstCellOfMerge = -1;
            var lastCellOfMerge = -1;
            // Iterate Columns (Scan down each column to collect up header info)
            for (int colIdx = 1; colIdx <= headerRows.First().Cells().Count(); colIdx++)
            {
                var headerText = new List<string>();
                bool expectedColumn = false;
                var rc = 0;
                foreach (var row in headerRows)
                {
                    var idx = row.RowNumber();
                    var cell = row.Cell(colIdx);

                    if (!cell.Value.IsText && !cell.Value.IsBlank)
                        throw new Exception($"Encountered a non Text value in header: Row={idx}, Column={colIdx}");

                    Debug.WriteLine($"Column {colIdx}, Row: {idx}: Value = {cell.Value}");

                    // Handle Merged Cells
                    if (IsMergedCell(cell, out var start, out var end))
                    {
                        insideMergedCell = true;
                        firstCellOfMerge = start;
                        lastCellOfMerge = end;
                    }
                    if (colIdx > lastCellOfMerge)
                    {
                        insideMergedCell = false;
                        firstCellOfMerge = -1;
                        lastCellOfMerge = -1;
                    }

                    // Get the header value
                    string textValue;
                    if (cell.Value.IsBlank || cell.Value.IsText && cell.Value.GetText() == "")
                    {
                        if (insideMergedCell && colIdx > firstCellOfMerge)
                            textValue = GetCellValueToLeft(headers, colIdx, rc);
                        else
                            textValue = "";
                    }
                    else
                    { // read as text if not blank
                        textValue = cell.Value.GetText();
                    }

                    textValue = StripIllegalCharacters(textValue);

                    expectedColumn = CheckAndHandleExpectedColumn(headers, colIdx, textValue);
                    if (!expectedColumn) 
                        headerText.Add(textValue);

                    rc++;
                }

                if (!expectedColumn)
                    headers.AddCableColumn(colIdx, headerText);
            }
        }

        private static string StripIllegalCharacters(string text)
        {
            return text.Replace("{", "").Replace("}", ""); // Strip curly brackets because they are used in code to define header components.

        }

        private static string GetCellValueToLeft(WorksheetHeaderData headerData, int col, int row)
        {
            var values = headerData.GetCableColumnValues(col - 1);
            if (values == null)
                return "";
            return values[row];
        }

        private static bool IsMergedCell(IXLCell cell, out int start, out int end)
        {
            start = -1;
            end = -1;

            if (!cell.IsMerged()) 
                return false;

            var mergedRange = cell.MergedRange();
            start = mergedRange.RangeAddress.FirstAddress.ColumnNumber;
            end = mergedRange.RangeAddress.LastAddress.ColumnNumber;

            Debug.WriteLine($"Inside Merged Cell: {start}->{end}");

            return true;
        }

        private static bool CheckAndHandleExpectedColumn(WorksheetHeaderData headers, int columnIndex, string textValue)
        {
            foreach (MetadataType expHeader in Enum.GetValues(typeof(MetadataType)))
            {
                var possibleValues = expHeader.GetStringArrayValue();
                for (int i = 0; i < possibleValues.Length; i++)
                {
                    // Any match of these values is enough (there should be no duplications)
                    if (textValue.StartsWith(possibleValues[i], StringComparison.OrdinalIgnoreCase))
                    {
                        headers.SetExpectedHeaderColumn(expHeader, columnIndex);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
