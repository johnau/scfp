using ClosedXML.Excel;
using FaceplateDataExtractor.Excel.Helper;
using FaceplateDataExtractor.Model;
using System.Diagnostics;
using System.Numerics;
using static FaceplateDataExtractor.Excel.MsExcelFaceplateDataExtractor;

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
        /// <summary>
        /// Replacement for <see cref="Vector2"> that uses ints instead of floats
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public record IntVec2(int X, int Y) { }
        /// <summary>
        /// Config for Excel Data Extractor
        /// </summary>
        /// <remarks>
        /// If <c>AutoDetect</c> is <c>False</c>, values must be provided for <c>HeaderStart</c>, <c>HeaderEnd</c>, <c>DataStart</c>, <c>DataEnd</c>
        /// </remarks>
        /// <param name="AutoDetect"></param>
        /// <param name="HeaderStart"></param>
        /// <param name="HeaderEnd"></param>
        /// <param name="DataStart"></param>
        /// <param name="DataEnd"></param>
        public record Configuration(bool AutoDetect, IntVec2? HeaderStart = null, IntVec2? HeaderEnd = null, IntVec2? DataStart = null, IntVec2? DataEnd = null) { }
        

        private Configuration _configuration;
        private string _filePath;
        private int _sheet;
        private WorksheetHeaderData _headerData;
        private List<WorksheetRowData> _rowDatas;
        private List<string> _errors;
        public bool HasErrors => _errors.Count > 0;
        public List<string> Errors => new List<string>(_errors);

        public MsExcelFaceplateDataExtractor(string filePath, int sheet, Configuration? config = null)
        {
            if (config == null)
                _configuration = new Configuration(AutoDetect: true);
            else
                _configuration = config;
            CheckConfig(); // throws exceptions for bad config

            _filePath = filePath;
            _sheet = sheet;
            _headerData = new WorksheetHeaderData(filePath, sheet);
            _rowDatas = [];
            _errors = [];
        }

        private void CheckConfig()
        {
            if (_configuration == null) 
                throw new Exception("Application Error: Configuration is null");

            if (!_configuration.AutoDetect)
            {
                if (_configuration.HeaderStart == null) throw new Exception("Configuration is set to Manual but no range values provided for `HeaderStart`");
                if (_configuration.HeaderEnd == null) throw new Exception("Configuration is set to Manual but no range values provided for `HeaderEnd`");
                if (_configuration.DataStart == null) throw new Exception("Configuration is set to Manual but no range values provided for `DataStart`");
                if (_configuration.DataEnd == null) throw new Exception("Configuration is set to Manual but no range values provided for `DataEnd`");
            }
        }

        public bool TryExtractData(int format, out List<ExtractedFaceplateData> data, out List<ExtractedFaceplateData> rejectedData)
        {
            data = [];
            rejectedData = [];

            var workbook = new XLWorkbook(_filePath);
            var worksheet = workbook.Worksheet(_sheet);
            var rows = worksheet.Rows();

            // Get Header and Data bounds either from Config or by Auto-Detection
            IntVec2 headerBoundsStart, headerBoundsEnd, dataBoundsStart, dataBoundsEnd;
            if (_configuration.AutoDetect)
            {
                (headerBoundsStart, headerBoundsEnd) = FindHeaderBounds(rows);
                (dataBoundsStart, dataBoundsEnd) = FindDataBounds(rows);
            } else
            {
                // using null-forgiveness here as we already checked the configuration during construction
                headerBoundsStart = _configuration.HeaderStart!; 
                headerBoundsEnd = _configuration.HeaderEnd!;
                dataBoundsStart = _configuration.DataStart!;
                dataBoundsEnd = _configuration.DataEnd!;
            }

            HeaderHelper.PopulateHeaderData(_headerData, worksheet.Rows(headerBoundsStart.Y, headerBoundsEnd.Y)); 

            Debug.WriteLine(_headerData.ToString());

            BodyHelper.PopulateRowDatas(_rowDatas, worksheet.Rows(dataBoundsStart.Y, dataBoundsEnd.Y), _headerData);

            var invalidDiscardedRows = BodyHelper.StripInvalidRows(_rowDatas, _headerData);
            // map invalidDiscardedRows to ExtractedFaceplateData object

            // Debug - delete
            foreach (var row in _rowDatas) {
                Debug.WriteLine($"Row {row.RowNumber}:");
                foreach (var dat in row.RowData)
                {
                    Debug.WriteLine($"-> {dat.Key}: {dat.Value}");
                }
            }

            return true;
        }

        /// <summary>
        /// Detect the range of the header
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private (IntVec2, IntVec2) FindHeaderBounds(IXLRows rows)
        {
            var start = new IntVec2(1, 2);
            var end = new IntVec2(51, 4);
            return (start, end);
        }

        /// <summary>
        /// Detect the range of the table data
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private (IntVec2, IntVec2) FindDataBounds(IXLRows rows)
        {
            var start = new IntVec2(1, 5);
            var end = new IntVec2(1, 104);
            return (start, end);
        }
    }
}
