
namespace FaceplateDataExtractor.Excel
{
    internal class WorksheetCellData
    {
        public int RowNumber { get; }
        public int ColumnNumber { get; }
        public string HeaderText { get; }
        public object Value { get; }

        public WorksheetCellData(string headerText, object value, int columnNumber, int rowNumber)
        {
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
            Value = value;
            HeaderText = headerText;
        }
    }
}
