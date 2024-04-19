using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using FaceplateDataExtractor.Excel.Helper;
using FaceplateDataExtractor.Model;
using FaceplateDataExtractor.Model.Mapper;
using FaceplateDataExtractor.Utility;
using System.Diagnostics;
using System.Numerics;
using static FaceplateDataExtractor.Excel.MsExcelFaceplateDataExtractor;

namespace FaceplateDataExtractor.Excel;

/// <summary>
/// Extracts faceplate data from a semi-predictable Excel worksheet
/// </summary>
/// <remarks>
/// Some assumptions are made:
/// - The first 5 columns are static and must be included in the template (order does not matter)
/// - Anchor points: "PANEL ID" - Must be located in the first column of header, and last row of header
/// - Data rows must have a "PANEL ID" to be ingested.
/// </remarks>
public class MsExcelFaceplateDataExtractor : IFaceplateDataExtractor
{
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="sheet"></param>
    /// <param name="config"></param>
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

    protected virtual void CheckConfig()
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

        var mapper = new ExtractedFaceplateDataMapper();
        data = mapper.Map(_rowDatas);
        return true;

        //Dictionary<string, ExtractedFaceplateData> faceplatesData = [];
        //// Debug - delete
        //foreach (var row in _rowDatas) {
        //    Debug.WriteLine($"Row {row.RowNumber}:");

        //    var cellList = row.RowData.ToList();
        //    //var model = new ExtractedFaceplateData();
        //    for (int i = 0; i < cellList.Count; i++)
        //    {
        //        // Iterate the cells in the row, accumulate the data to go into the ExtractedFaceplateData object
        //        var cellData = cellList[i].Value;
        //        //var headerTextString = StringsHelper.ListToString(cellData.HeaderText).ToLower();

        //        if (cellData.Value == null || cellData.Value + "" == string.Empty)
        //        {
        //            Debug.WriteLine($"Ignored this cell as it is empty (Cell={cellData.ColumnNumber}, Row={cellData.RowNumber})");
        //            continue;
        //        }

        //        //var panelIdTitle = PanelDescriptorDataType.PANEL_ID.GetStringArrayValue()[0].ToLower();
        //        //var descriptionTitle = PanelDescriptorDataType.DESCRIPTION.GetStringArrayValue()[0].ToLower();
        //        //var locationTitle = PanelDescriptorDataType.LOCATION.GetStringArrayValue()[0].ToLower();
        //        //var roomTitle = PanelDescriptorDataType.LOCATION.GetStringArrayValue()[0].ToLower();
        //        //var afflTitle = PanelDescriptorDataType.AFFL.GetStringArrayValue()[0].ToLower();
        //        //if (headerTextString.Contains(panelIdTitle))
        //        //{
        //        //    model.PanelId = cellData.Value + "";
        //        //    continue;
        //        //}
        //        //if (headerTextString.Contains(descriptionTitle)) 
        //        //{ 
        //        //    model.Description = cellData.Value + "";
        //        //    continue;
        //        //}
        //        //if (headerTextString.Contains(locationTitle))
        //        //{
        //        //    model.Location = cellData.Value + "";
        //        //    continue;
        //        //}
        //        //if (headerTextString.Contains(roomTitle))
        //        //{
        //        //    model.Room = cellData.Value + "";
        //        //    continue;
        //        //}
        //        //if (headerTextString.Contains(afflTitle))
        //        //{
        //        //    model.AboveFinishedFloorLevel = cellData.Value + "";
        //        //    continue;
        //        //}

        //        //Debug.WriteLine($"{model}");
                
        //        Debug.WriteLine($"-> {StringsHelper.ListToString(cellData.HeaderText)}: {cellData.Value}");

        //        //var cableSystemData = new CableSystemData();

        //        ////faceplatesData.TryAdd(cellData.HeaderText.First(), new ExtractedFaceplateData());

        //        //// Not super pretty, but at this point we are at the cable system columns
        //        //// This heavily relies in the structure of the able that should not change
        //        //// lookahead to next related rows 
        //        //// we expect this loop to exit quite quickly every time (on first or second iteration)
        //        //var cableSystemQuantity = 0;
        //        //var cableSystemDestination = "";
        //        //for (int ii = i + 1; ii < cellList.Count; ii++)
        //        //{
        //        //    var nextCellData = cellList[ii].Value;
        //        //    if (cellData.HeaderText[0] != nextCellData.HeaderText[0])
        //        //    {
        //        //        Debug.WriteLine($"The first part of header was not matched in the next column: {cellData.HeaderText[0]} -> Handling next...");
        //        //        break;
        //        //    }

        //        //    //// check the second header part if there is one
        //        //    //// probably not neccessary
        //        //    //if (cellData.HeaderText.Count > 1)
        //        //    //{
        //        //    //    if (cellData.HeaderText[1] != nextCellData.HeaderText[1])
        //        //    //    {
        //        //    //        Debug.WriteLine($"The second part of header was not matched in the next column: {cellData.HeaderText[1]} -> Handling next...");
        //        //    //        break;
        //        //    //    }
        //        //    //}

        //        //    if (nextCellData.Value == null || nextCellData.Value + "" == string.Empty)
        //        //    {
        //        //        // shouldn't occur
        //        //        throw new Exception($"The previous cell in this system group: {StringsHelper.ListToString(nextCellData.HeaderText)} had a value, but this cell does not (Cell={cellData.ColumnNumber}, Row={cellData.RowNumber})");
        //        //    }


        //        //}

        //        //cableSystemData.Quantity = cableSystemQuantity;
        //        //cableSystemData.Destination = cableSystemDestination;
        //    }
        //}

        //return true;
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
