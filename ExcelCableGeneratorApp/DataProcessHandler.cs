using DocumentFormat.OpenXml.Spreadsheet;
using ExcelCableGeneratorApp.Extract.Aggregates;
using ExcelCableGeneratorApp.Extract.SimpleExtractor;
using ExcelCableGeneratorApp.Sorting.Aggregates;
using ExcelCableGeneratorApp.Sorting;
using ExcelCableGeneratorApp.Identifier;
using ExcelCableGeneratorApp.Identifier.Aggregates;
using System.Reflection.Emit;
using System.Diagnostics;
using ExcelCableGeneratorApp.Utility;

namespace ExcelCableGeneratorApp;

internal class DataProcessHandler
{
    private readonly string _filePath;
    private readonly int _sheetNumber;
    private List<string> _primaryHeaders;
    private Dictionary<string, string> _systemIdMapping;
    private List<SystemCableData> _cableData;
    private List<SystemCableData> _filteredCableData;
    private List<SystemCableGroup> _sortedCableData;
    private List<IdentifiedCableGroup> _identifiedData;
    private IdentifierGenerator IdGenerator;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="sheetNumber"></param>
    public DataProcessHandler(string filePath, int sheetNumber)
    {
        _filePath = filePath;
        _sheetNumber = sheetNumber;
        _primaryHeaders = [];
        _systemIdMapping = [];
        _cableData = [];
        _filteredCableData = [];
        _sortedCableData = [];
        _identifiedData = [];
        IdGenerator = new IdentifierGenerator();
    }

    /// <summary>
    /// Load settings from json file
    /// </summary>
    public void LoadSettings(string settingsFileName)
    {
        List<string> primaryHeaders = ["Panel Id", "Description", "Location", "Room", "AFFL"];
        _primaryHeaders = primaryHeaders;

        Dictionary<string, string> systemIdMapping = new(){
            {"TECHNICAL DATA", "TD" },
            {"MULTIMODE FIBER", "MF" },
            {"VIDEO TIE LINE", "VTL" },
            {"DIGITAL MEDIA", "DM" },
            {"AV CONTROL", "AVC" },
            {"AUDIO DIGITAL / ANALOGUE", "A" },
            {"ETHERNET AUDIO (Dante)", "DA" },
            {"TALKBACK", "TB" },
            {"PERFORMANCE RELAY INPUT", "PA?" },
            {"PAGING STATION", "PS" },
            {"PAGING VOLUME CONTROL", "PVC" },
            {"PAGING SPEAKER", "PSP" },
            {"PERFORMANCE LOUDSPEAKER", "PA" },
            {"STAGE LIGHTING CONTROL (DMX)", "DMX" },
            {"BLUE / WORK LIGHT CONTROL", "WLC" },
            {"HOIST CONTROL", "MX" },
            {"HOUSE CURTAIN CONTROL", "HC" },
            {"PENDENT CONTROL (WITH ESTOP)", "PC" },
            {"ESTOP", "ES" },
            {"STAGE LIGHTING OUTLETS SINGLE 10A", "SLO" },
            {"BLUE/WORK LIGHT OUTLETS", "WLO" },
            {"10A 'DIRTY' GPO DOUBLE OUTLET", "DGPO" },
            {"10A AUDIO POWER DOUBLE OUTLET", "AGPU" },
            {"3 PHASE OUTLET", "3PO" },
        };
        var sanitizedSystemIdMapping = systemIdMapping.ToDictionary(
            kvp => StringHelper.Sanitize(kvp.Key), // must to sanitize - strings from the spreadsheet are also sanitized with this method
            kvp => kvp.Value
        );
        _systemIdMapping = sanitizedSystemIdMapping;

        foreach (var item in _systemIdMapping)
        {
            IdGenerator.StartNewSequence(item.Value);
            Debug.WriteLine($"Started ID Sequence for {item.Key} : '{item.Value}000'");
        }
    }

    /// <summary>
    /// Process xls file for cable data
    /// </summary>
    public List<SystemCableData> ProcessFile()
    {
        var config = new ExcelCableCrawler.Configuration(_primaryHeaders);
        var crawler = new ExcelCableCrawler(config);

        // read in raw data from the table
        _cableData = crawler.CrawlCableTable(_filePath, _sheetNumber);

        return _cableData;
    }

    /// <summary>
    /// Filter data without panel id, description, or location (all are required to be a valid record) (this filter should stay at top level)
    /// </summary>
    /// <returns></returns>
    public List<SystemCableData> FilterData()
    {
        _filteredCableData = _cableData
            .Where(data => !string.IsNullOrWhiteSpace(data.PanelId) && !string.IsNullOrWhiteSpace(data.Description) && !string.IsNullOrWhiteSpace(data.Location))
            .Select(data => data.CreateSanitizedObject())
            .ToList();

        return _filteredCableData;
    }

    /// <summary>
    /// Sort the data (group by system type) -> sort by destination rack priority (highest qty on rack) -> then by 
    /// </summary>
    public List<SystemCableGroup> SortData()
    {
        var cableSystemGroups = SortHelper.GroupBySystem(_filteredCableData);
        foreach (var group in cableSystemGroups)
        {
            var sortedCables = SortHelper.SortSystemGroup(group.Cables);
            _sortedCableData.Add(new SystemCableGroup(group.Name, sortedCables));
        }

        return _sortedCableData;
    }

    /// <summary>
    /// Assign id's to data
    /// </summary>
    /// <remarks>
    /// Batches of 24, ended by rack change
    /// </remarks>
    /// <returns></returns>
    public List<IdentifiedCableGroup> AssignIdsToCables()
    {
        foreach (var group in _sortedCableData)
        {
            List<IdentifiedCable> identifiedCables = [];

            var idPrefix = MatchSystemIdMapping(group.Name);
            if (string.IsNullOrEmpty(idPrefix))
            {
                throw new Exception($"Unable to map: {group.Name}");
            }
            foreach (var cable in group.Cables)
            {
                for (int i = 0; i < cable.Quantity; i++)
                {
                    var (nextId, nextIdNumber) = IdGenerator.NextId(idPrefix, cable.DestinationId);
                    var identifier = new StageCraftCableId(idPrefix, nextIdNumber);
                    identifiedCables.Add(new IdentifiedCable(identifier, cable));
                }
            }

            _identifiedData.Add(new IdentifiedCableGroup(group.Name, identifiedCables));
        }

        return _identifiedData;
    }

    private string MatchSystemIdMapping(string systemName)
    {
        var matchedPair = _systemIdMapping.FirstOrDefault(sys => systemName.Contains(sys.Key, StringComparison.OrdinalIgnoreCase));
        return matchedPair.Value;
    }
}
