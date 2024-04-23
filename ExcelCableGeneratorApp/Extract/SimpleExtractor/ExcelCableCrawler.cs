using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelCableGeneratorApp.Extract.Aggregates;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace ExcelCableGeneratorApp.Extract.SimpleExtractor;

public class ExcelCableCrawler
{
    public static readonly string ToFromLabel = "To/From";
    public static readonly string ZoneLabel = "Zone";
    public static readonly string QuantityLabel = "Quantity";
    public record Configuration(List<string> PrimaryHeaders, int HeaderRowPrimary = 4, int HeaderRowCableType = 3, int HeaderRowSystem = 2, int DataStartRow = 6) { }

    private readonly int _headerRowPrimary; // Has Panel Id, Description, etc and Quantity + To/From columns headers
    private readonly int _headerRowCableType; // Has cable type headers
    private readonly int _headerRowSystem; // Has system name headers
    private readonly int _dataStartRow;
    private readonly Dictionary<string, int> _primaryDataColumns;
    private readonly Dictionary<(string, string), (Dictionary<string, int>, (string, int))> _systemDataColumns; // <Key> is Tuple of System Type and Cable Type, <Value> is a Tuple of a Dictionary of Col Names and Indexes for the Quantity Columns and Tuple of Col Name and Index for the To/From column

    Func<IXLCell, string, bool> TextCellContains = (IXLCell cell, string s) => cell.Value.IsText && cell.Value.GetText().Contains(s, StringComparison.OrdinalIgnoreCase);

    public ExcelCableCrawler(Configuration config)
    {
        _headerRowPrimary = config.HeaderRowPrimary;
        _headerRowCableType = config.HeaderRowCableType;
        _headerRowSystem = config.HeaderRowSystem;
        _dataStartRow = config.DataStartRow;
        _primaryDataColumns = config.PrimaryHeaders.ToDictionary(str => str, _ => 0);
        _systemDataColumns = [];
    }

    public List<SystemCableData> CrawlCableTable(string filePath, int sheet)
    {
        var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheet(sheet);
        var primaryHeaderRow = worksheet.Rows(_headerRowPrimary, _headerRowPrimary).First();
        var cableTypeHeaderRow = worksheet.Rows(_headerRowCableType, _headerRowCableType).First();
        var systemHeaderRow = worksheet.Rows(_headerRowSystem, _headerRowSystem).First();

        var dataRows = worksheet.Rows(_dataStartRow, worksheet.LastRowUsed().RowNumber());

        CrawlHeaders(primaryHeaderRow, cableTypeHeaderRow, systemHeaderRow);
        var data = CrawlData(dataRows);

        return data;
    }

    private void CrawlHeaders(IXLRow primaryHeaderRow, IXLRow cableTypeHeaderRow, IXLRow systemHeaderRow)
    {
        // Get column numbers for the primary headers (i.e. "Panel ID", "Description", "Location", etc)
        var currentGroupQtyCols = new Dictionary<string, int>();
        var lastSystemType = "";
        var lastCableType = "";

        var currentSystemType = "";
        var currentCableType = "";

        foreach (var xlCell in primaryHeaderRow.Cells())
        {
            // Primary columns must be found - else never process the rest of the table
            if (_primaryDataColumns.Where(kvp => kvp.Value == 0).Any())
            {
                foreach (var pCol in _primaryDataColumns)
                {
                    if (xlCell.Value.IsText && xlCell.Value.GetText().Contains(pCol.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        _primaryDataColumns[pCol.Key] = xlCell.Address.ColumnNumber;
                        break;
                    }
                }
                continue;
            }

            // Search for the Quantity, Quantity Send, Quantity Return, To/From, etc headers and map their locations against System and Cable types.
            if (TextCellContains(xlCell, QuantityLabel.ToLower()))
            {
                // look in other header rows to get system type and column type
                var systemTypeCell = systemHeaderRow.Cell(xlCell.Address.ColumnNumber);
                var cableTypeCell = cableTypeHeaderRow.Cell(xlCell.Address.ColumnNumber);
                if (!systemTypeCell.IsEmpty() && systemTypeCell.Value.IsText)
                {
                    lastSystemType = currentSystemType;
                    currentSystemType = systemTypeCell.Value.GetText();
                }
                if (!cableTypeCell.IsEmpty() && cableTypeCell.Value.IsText)
                {
                    lastCableType = currentCableType;
                    currentCableType = cableTypeCell.Value.GetText();
                }

                // Try to add the col to the Quantities group,
                //  >> if fail, the last column must be Quantity without a To/From or Zone column (ie. a solo quantity column)
                //  >>>> in this case we persist the previous column
                if (!currentGroupQtyCols.TryAdd(xlCell.Value.GetText(), xlCell.Address.ColumnNumber)) 
                {
                    // push the last group
                    var qtyCols_Copy = currentGroupQtyCols.ToDictionary(entry => entry.Key, entry => entry.Value);
                    _systemDataColumns.Add((lastSystemType, lastCableType), (qtyCols_Copy, ("", -1)));

                    // cleanup and prepare for another group
                    currentGroupQtyCols.Clear();
                    currentGroupQtyCols.Add(xlCell.Value.GetText(), xlCell.Address.ColumnNumber);
                }
            }
            // If we encounter a To/From or Zone header we are at the end of a column group, persist the group with SystemType and
            // column type (which should be found on the first column of the group).
            else if (TextCellContains(xlCell, ToFromLabel.ToLower()) || TextCellContains(xlCell, ZoneLabel.ToLower()))
            {
                // Ignore if we dont have a system type
                if (string.IsNullOrEmpty(currentSystemType))
                {
                    Debug.WriteLine($"The column group could not be read due to missing System Type ('{currentSystemType}'), it was skipped");
                    continue;
                }

                var qtyCols_Copy = currentGroupQtyCols.ToDictionary(entry => entry.Key, entry => entry.Value);
                var toFromCol = (xlCell.Value.GetText(), xlCell.Address.ColumnNumber);
                _systemDataColumns.Add((currentSystemType, currentCableType), (qtyCols_Copy, toFromCol));

                // cleanup each time we arrive at to/from col
                currentGroupQtyCols.Clear();
                currentSystemType = "";
                currentCableType = "";
            }
        }

        // add the last item if there is one
        if (currentGroupQtyCols.Count > 0) {
            var remainingCopy = currentGroupQtyCols.ToDictionary(entry => entry.Key, entry => entry.Value);
            if (!_systemDataColumns.TryAdd((currentSystemType, currentCableType), (remainingCopy, ("", -1))))
            {
                Debug.WriteLine($"Could not add the last column: {currentSystemType}");
            }
        }

        WriteDebugPrimaryDataColumns();
    }

    private List<SystemCableData> CrawlData(IXLRows dataRows)
    {
        var cableData = new List<SystemCableData>();
        foreach (var xlRow in dataRows)
        {
            // Collect primary cell values for this row
            Dictionary<string, string> primaryCellValues = _primaryDataColumns.Select(kvp => kvp.Key).ToDictionary(str => str, _ => ""); // prepopulate values dict
            foreach (var primaryColumn in _primaryDataColumns)
            {
                var cellValue = xlRow.Cell(primaryColumn.Value).Value;

                var cellText = "";
                if (cellValue.IsText) cellText = cellValue.GetText();
                else if (cellValue.IsNumber) cellText = cellValue.GetNumber() + "";

                primaryCellValues[primaryColumn.Key] = cellText;
            }

            // Collect cable values
            Dictionary<(string, string), Dictionary<string, string>> systemCellValues = _systemDataColumns.Select(kvp => kvp.Key).ToDictionary(str => str, _ => new Dictionary<string, string>()); // prepopulate values dict
            foreach (var systemColumn in _systemDataColumns)
            {
                var (qtyCols, (_, toFromColIndex)) = systemColumn.Value;

                foreach (var qtyCol in qtyCols)
                {
                    var qtyName = qtyCol.Key;
                    var cell = xlRow.Cell(qtyCol.Value);

                    var textCellValue = GetCellValueAsString(cell);
                    if (string.IsNullOrEmpty(textCellValue))
                        continue;

                    if (!systemCellValues[systemColumn.Key].TryAdd(qtyName, textCellValue))
                        throw new Exception("Duplicate quantity name should not occur...");
                }

                var toFromValue = "";
                if (toFromColIndex != -1)
                    toFromValue = GetCellValueAsString(xlRow.Cell(toFromColIndex));

                if (!string.IsNullOrEmpty(toFromValue))
                    systemCellValues[systemColumn.Key].TryAdd(ToFromLabel, toFromValue);
            }

            var _cableData = ProcessData(primaryCellValues, systemCellValues);
            cableData.AddRange(_cableData);
        }

        //foreach (var cd in cableData)
        //{
        //    Debug.WriteLine(cd);
        //}

        return cableData;
    }

    /// <summary>
    /// Filter empty data and map data to the <see cref="SystemCableData"> object.
    /// </summary>
    /// <param name="primaryCellValues"></param>
    /// <param name="systemCellValues"></param>
    /// <returns></returns>
    private List<SystemCableData> ProcessData(Dictionary<string, string> primaryCellValues, Dictionary<(string, string), Dictionary<string, string>> systemCellValues)
    {
        var cableData = new List<SystemCableData>();

        foreach (var cv in systemCellValues)
        {
            var (sysType, cableType) = cv.Key;
            var colValues = cv.Value;
            if (colValues.Count == 0) continue; // skip empty

            var destination = "";
            if (colValues.TryGetValue(ToFromLabel, out var toFromValue))
            {
                destination = toFromValue;
                colValues.Remove(ToFromLabel);
            }

            foreach (var qtyCol in colValues)
            {
                var panelId = primaryCellValues.FirstOrDefault(kv => string.Equals(kv.Key.Trim(), "panel id", StringComparison.OrdinalIgnoreCase)).Value;
                var description = primaryCellValues.FirstOrDefault(kv => string.Equals(kv.Key.Trim(), "description", StringComparison.OrdinalIgnoreCase)).Value;
                var location = primaryCellValues.FirstOrDefault(kv => string.Equals(kv.Key.Trim(), "location", StringComparison.OrdinalIgnoreCase)).Value;
                var room = primaryCellValues.FirstOrDefault(kv => string.Equals(kv.Key.Trim(), "room", StringComparison.OrdinalIgnoreCase)).Value;
                var affl = primaryCellValues.FirstOrDefault(kv => string.Equals(kv.Key.Trim(), "affl", StringComparison.OrdinalIgnoreCase)).Value;

                var qty = 0;
                if (int.TryParse(qtyCol.Value, out var intQty))
                    qty = intQty;
                else
                    qty = ExtractFirstNumberSequence(qtyCol.Value);

                // need to store these, probably an error, dont want to discard
                //if (qty == 0)
                //    continue;

                // need to store system name, cable type, qty type etc
                var scd = new SystemCableData(panelId, description, location, room, affl, sysType, cableType, qtyCol.Key, qty, destination);
                cableData.Add(scd);
            }
        }

        return cableData;
    }

    /// <summary>
    /// Used to extract number value from cell that may contain a number and then some text (ie. NOTE 7)
    /// </summary>
    /// <remarks>
    /// Do not use this for truncating RackIds or PanelIds or anything of that nature.
    /// </remarks>
    /// <param name="s"></param>
    /// <returns></returns>
    private static int ExtractFirstNumberSequence(string s)
    {
        string result = "";
        foreach (char c in s)
        {
            if (Char.IsDigit(c))
                result += c;
            else if (result.Length > 0 && !Char.IsDigit(c))
                break;
        }
        if (result.Length > 0 && int.TryParse(result, out var intValue))
            return intValue;

        return 0;
    }

    private static string GetCellValueAsString(IXLCell cell)
    {
        var textCellValue = "";
        if (cell.Value.IsBlank)
            return "";
        else if (cell.Value.IsText)
            textCellValue = cell.Value.GetText();
        else if (cell.Value.IsNumber)
            textCellValue = cell.Value.GetNumber() + "";
        else
        {
            Debug.WriteLine($"Unhandled cell data type: {cell.Value.Type}");
            return "";
        }

        return textCellValue;
    }

    private void WriteDebugPrimaryDataColumns()
    {
        // debug
        if (_primaryDataColumns.Where(kvp => kvp.Value == 0).Any())
            throw new Exception($"Could not find all Primary Headers ({string.Join(", ", _primaryDataColumns.Keys)}) in the table at row {_headerRowPrimary}");

        foreach (var sysCol in _systemDataColumns)
        {
            var (systemType, cableType) = sysCol.Key;
            var (qtyColsDict, (_, toFromColNumber)) = sysCol.Value;

            Debug.WriteLine($"System Column Group: {systemType} - {cableType}");
            foreach (var qtyCol in qtyColsDict)
            {
                Debug.WriteLine($">> Qty Col: {qtyCol.Key} @ {qtyCol.Value}");
            }
            Debug.WriteLine($">> To/From Col: {toFromColNumber}");
            Debug.WriteLine("---------");
        }
    }
}
