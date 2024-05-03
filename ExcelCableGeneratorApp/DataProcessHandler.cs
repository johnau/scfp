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
using System.Text.RegularExpressions;
using ExcelCableGeneratorApp.Dxf.Drawing.Element;
using ExcelCableGeneratorApp.Convert;
using ExcelCableGeneratorApp.Dxf;
using netDxf;
using ExcelCableGeneratorApp.Dxf.Drawing.Helper;
using ExcelCableGeneratorApp.Dxf.Aggregates.Data;

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
    private List<IdentifiedCableGroup> _cablesDataBySource;
    private List<IdentifiedCableGroup> _cablesDataByDestination;
    private List<IdentifiedCableGroup> _cablesDataByRoomLocation;

    private IdentifierGenerator IdGenerator;
    private TechPanelBuilder Builder;



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
        _cablesDataBySource = [];
        _cablesDataByDestination = [];
        _cablesDataByRoomLocation = [];

        IdGenerator = new IdentifierGenerator();
        Builder = new TechPanelBuilder();
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
        // may throw exception
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
            //var locationInnerGroup = SortHelper.GroupByLocation(group.Cables);

            var sortedCables = SortHelper.SortSystemGroup(group.Cables);
            _sortedCableData.Add(new SystemCableGroup(group.Name, sortedCables));
        }

        return _sortedCableData;
    }

    /// <summary>
    /// Assign id's to data - Grouped by the system type
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

    public List<IdentifiedCableGroup> GroupByDestinationAcrossEntireSystem()
    {
        var allCables = _identifiedData.SelectMany(id => id.Cables).ToList(); // this is bad, we can create some indexes that store the cables in various ways
        var groupedByRack = SortHelper.GroupByDest(allCables);
        foreach (var rackGroup in groupedByRack) 
        {
            Debug.WriteLine($"Rack Group: {rackGroup.Name}");
            foreach (var cable in rackGroup.Cables)
            {
                Debug.WriteLine($"-->> {cable}");
            }
            _cablesDataByDestination.Add(rackGroup);
        }

        return _cablesDataByDestination;
    }

    public List<IdentifiedCableGroup> GroupBySourceAcrossEntireSystem()
    {
        var allCables = _identifiedData.SelectMany(grp => grp.Cables).ToList(); // this is bad, we can create some indexes that store the cables in various ways
        var groupedBySourcePanelId = SortHelper.GroupBySource(allCables);
        foreach (var panelGroup in groupedBySourcePanelId)
        {
            Debug.WriteLine($"Rack Group: {panelGroup.Name}");
            foreach (var cable in panelGroup.Cables)
            {
                Debug.WriteLine($"-->> {cable}");
            }
            _cablesDataBySource.Add(panelGroup);
        }

        return _cablesDataBySource;
    }

    private string MatchSystemIdMapping(string systemName)
    {
        var matchedPair = _systemIdMapping.FirstOrDefault(sys => systemName.Contains(sys.Key, StringComparison.OrdinalIgnoreCase));
        return matchedPair.Value;
    }

    public List<IdentifiedCableGroup> GroupByRoomOrLocationAcrossEntireSystem()
    {
        var allCables = _identifiedData.SelectMany(id => id.Cables).ToList(); // this is bad, we can create some indexes that store the cables in various ways
        var groupedByRoomOrLocation = SortHelper.GroupByRoomOrLocation(allCables);
        foreach (var roomGroup in groupedByRoomOrLocation)
        {
            Debug.WriteLine($"Rack Group: {roomGroup.Name}");
            foreach (var cable in roomGroup.Cables)
            {
                Debug.WriteLine($"-->> {cable}");
            }
            _cablesDataByRoomLocation.Add(roomGroup);
        }

        return _cablesDataByRoomLocation;
    }

    public Dictionary<string, List<FixedGridPanel>> GenerateTechPanels(List<IdentifiedCableGroup> groupedBySource)
    {
        Dictionary<string, List<FixedGridPanel>> builtPanels = [];

        Directory.CreateDirectory("./generated_tech_panels");
        var socketInformation = CableToSocketConverter.ConvertCableGroupsToSockets(groupedBySource);

        foreach (var source in socketInformation)
        {
            Console.WriteLine($"Iterating source panel: {source.SourcePanelId}");

            var filteredSystemGroups = FilterHelper.FilterSystemGroupContentsForTechPanels(source.SystemGroups);
            if (filteredSystemGroups.Count == 0)
                continue;

            var panelsFit = TechPanelHelper.FitToPanel(filteredSystemGroups.SelectMany(g => g.Sockets).Count());
            // This part that produces panels only when the whoel thing fits on one can be removed, was just to quickly have a look at some results
            if (panelsFit.Count > 0) // if we have any that fit, we will just take the first one (largest)
            {
                // we can fit this whole source panel onto a single TP
                Console.WriteLine($"This source panel can fit onto one panel (smallest fit = {panelsFit[0]})");

                // create this panel...
                var builtPanel = Builder.BuildFixedGridPanel(panelsFit[0], source.SourcePanelId, filteredSystemGroups);
                if (builtPanels.TryGetValue(source.SourcePanelId, out var panelsList))
                {
                    panelsList.Add(builtPanel);
                } else
                {
                    var success = builtPanels.TryAdd(source.SourcePanelId, [builtPanel]);
                }
                // check if panel can be split down further.
                //DxfDocument doc = new();
                //builtPanel.Draw(doc.Entities);
                //doc.Save($"./generated_tech_panels/{source.SourcePanelId}.dxf");
            }

            var panelCombinations = TechPanelFitCalculator.CalculatePanelCombinations2(filteredSystemGroups);
            var leftOverSystemGroups = filteredSystemGroups;
            foreach (var combination in panelCombinations)
            {
                var firstSizeKvp = combination.PanelSizes.First();
                var panelSize = firstSizeKvp.Key;
                var qtyOf = firstSizeKvp.Value;

                var groupsForThisPanel = TechPanelFitCalculator.SelectGroupsForPanel(leftOverSystemGroups, panelSize.GetSocketQuantity(), 2, out var leftOver);

                leftOverSystemGroups = leftOver;

                var builtPanel = Builder.BuildFixedGridPanel(panelSize, source.SourcePanelId, groupsForThisPanel);
                if (builtPanels.TryGetValue(source.SourcePanelId, out var panelsList))
                {
                    panelsList.Add(builtPanel);
                }
                else
                {
                    var success = builtPanels.TryAdd(source.SourcePanelId, [builtPanel]);
                }
                //builtPanels.Add(builtPanel);
            }

            if (leftOverSystemGroups.Count > 0)
            {
                Debug.WriteLine($"There are still {leftOverSystemGroups.Count} system groups not on a panel");
                //throw new Exception("Need to handle this - and would be better to handle earlier in this process");
            }

            //foreach (var system in source.SystemGroups)
            //{
            //    Console.WriteLine($"System: {system.SystemName}");

            //    //foreach (var socket in system.Sockets)
            //    //{
            //    //    Console.WriteLine($"{socket.Key} - {socket.Value}");
            //    //}
            //}

        }

        return builtPanels;
    }
}
