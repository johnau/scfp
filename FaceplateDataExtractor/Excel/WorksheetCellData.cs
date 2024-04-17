
namespace FaceplateDataExtractor.Excel
{
    internal class WorksheetCellData
    {
        public int RowNumber { get; }
        public int ColumnNumber { get; }
        private List<string> _headerText;
        public List<string> HeaderText => new List<string>(_headerText);
        public object Value { get; }

        /// <summary>
        /// Main constructor, header text is stored a lsit
        /// </summary>
        /// <param name="headerText"></param>
        /// <param name="value"></param>
        /// <param name="columnNumber"></param>
        /// <param name="rowNumber"></param>
        public WorksheetCellData(List<string> headerText, object value, int columnNumber, int rowNumber)
        {
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
            Value = value;
            _headerText = headerText;
        }

        /// <summary>
        /// Deprecated constructor - HeaderText is no longer stored as a string
        /// </summary>
        /// <param name="headerText"></param>
        /// <param name="value"></param>
        /// <param name="columnNumber"></param>
        /// <param name="rowNumber"></param>
        public WorksheetCellData(string headerText, object value, int columnNumber, int rowNumber)
        {
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
            Value = value;
            _headerText = [headerText];
        }
    }
}
