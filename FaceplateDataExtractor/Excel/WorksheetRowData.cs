using System.Diagnostics;

namespace FaceplateDataExtractor.Excel
{
    internal class WorksheetRowData
    {
        public int RowNumber { get; }
        private Dictionary<string, WorksheetCellData> _rowData; //Data stored by header string

        /// <value>
        /// RowData property returns Dictionary of objects. 
        /// </value>
        /// <remarks>
        /// Object type is either string or double.
        /// </remarks>
        public Dictionary<string, WorksheetCellData> RowData
        {
            get => new Dictionary<string, WorksheetCellData>(_rowData);
        }

        public WorksheetRowData(int rowNumber)
        {
            RowNumber = rowNumber;
            _rowData = [];
        }

        public void AddRowData(string headerText, string value, int col, int row)
        {
            if (double.TryParse(value, out var doubleValue))
                AddRowDataObject(headerText, doubleValue, col, row);
            else
                AddRowDataObject(headerText, value, col, row);
        }

        public void AddRowData(string headerText, double value, int col, int row)
        {
            AddRowDataObject(headerText, value, col, row);
        }

        private void AddRowDataObject(string headerText, object value, int col, int row)
        {
            var cellData = new WorksheetCellData(headerText, value, col, row);
            if (!_rowData.TryAdd(headerText, cellData))
            {
                throw new ArgumentException($"Tried to add a duplicate column: {headerText}");
            }
        }
    }
}
