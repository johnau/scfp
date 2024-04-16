using ClosedXML.Excel;
using FaceplateDataExtractor.Excel.Helper;
using FaceplateDataExtractor.Model;
using System.Diagnostics;
using System.Numerics;

namespace FaceplateDataExtractor.Excel
{
    /// <summary>
    /// Extracts faceplate data from a semi-predictable Excel worksheet
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some assumptions are made:
    /// - The first 5 columns are static and must be included in the template (order does not matter)
    /// - Anchor points: "PANEL ID" <- Must be located in the first column of header, and last row of header
    /// - Data rows must have a "PANEL ID" to be ingested.
    /// </para>
    /// </remarks>
    public class MsExcelFaceplateDataExtractor : IFaceplateDataExtractor
    {
        private string _filePath;
        private int _sheet;
        private WorksheetHeaderData _headerData;
        private List<WorksheetRowData> _rowDatas;
        private List<string> _errors;
        public bool HasErrors => _errors.Count > 0;
        public List<string> Errors => new List<string>(_errors);

        public MsExcelFaceplateDataExtractor(string filePath, int sheet)
        {
            _filePath = filePath;
            _sheet = sheet;
            _headerData = new WorksheetHeaderData(filePath, sheet);
            _rowDatas = [];
            _errors = [];
        }

        public bool TryExtractData(int format, out List<ExtractedFaceplateData> data, out List<ExtractedFaceplateData> rejectedData)
        {
            data = [];
            rejectedData = [];

            var workbook = new XLWorkbook(_filePath);
            var worksheet = workbook.Worksheet(_sheet);

            var rows = worksheet.Rows();
            var (headerBoundsStart, headerBoundsEnd)  = FindHeaderBounds(rows);
            var (dataBoundsStart, dataBoundsEnd) = FindDataBounds(rows);
            HeaderHelper.PopulateHeaderData(_headerData, worksheet.Rows((int)headerBoundsStart.Y, (int)headerBoundsEnd.Y)); 

            Debug.WriteLine(_headerData.ToString());

            BodyHelper.PopulateRowDatas(_rowDatas, worksheet.Rows((int)dataBoundsStart.Y, (int)dataBoundsEnd.Y), _headerData);

            var invalidDiscardedRows = BodyHelper.StripInvalidRows(_rowDatas, _headerData);
            // map invalidDiscardedRows to ExtractedFaceplateData object

            // Debug
            foreach (var row in _rowDatas) {
                Debug.WriteLine($"Row {row.RowNumber}:");
                foreach (var dat in row.RowData)
                {
                    Debug.WriteLine($"-> {dat.Key}: {dat.Value}");
                }
            }

            return true;
        }

        private (Vector2, Vector2) FindHeaderBounds(IXLRows rows)
        {
            var start = new Vector2(1, 2);
            var end = new Vector2(51, 4);
            return (start, end);
        }

        private (Vector2, Vector2) FindDataBounds(IXLRows rows)
        {
            var start = new Vector2(1, 5);
            var end = new Vector2(1, 104);
            return (start, end);
        }
    }
}
