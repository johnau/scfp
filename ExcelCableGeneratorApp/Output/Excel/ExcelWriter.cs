using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelCableGeneratorApp.Identifier.Aggregates;
using System.Text.RegularExpressions;

namespace ExcelCableGeneratorApp.Output.Excel;

internal class ExcelWriter
{
    private readonly string _filePath;
    private readonly string _fileName;
    private readonly Dictionary<string, int> _columns;

    public ExcelWriter(string filePath, string fileName)
    {
        _filePath = filePath;
        _fileName = fileName;
        _columns = [];
    }

    public string FilePath => Path.GetFullPath(Path.Combine(_filePath, _fileName + ".xlsx"));

    public bool CreateWorkbook()
    {
        using var workbook = new XLWorkbook();
        workbook.AddWorksheet("x");
        workbook.SaveAs(FilePath);
        return true;
    }

    public bool FinalizeWorkbook()
    {
        using var workbook = new XLWorkbook(FilePath);
        workbook.Worksheets.Delete("x");
        workbook.Save();
        return true;
    }

    public bool WriteCablesToSpreadsheet(string systemName, List<IdentifiedCable> cables)
    {
        _columns.Add("Cable Id", 1);
        _columns.Add("Panel Id", 2);
        _columns.Add("Description", 3);
        _columns.Add("Location", 4);
        _columns.Add("Room", 5);
        _columns.Add("AFFL (mm)", 6);

        // check cable.QuantityType ... if contains Return Quantity or Send Quantity (load from settings) then we make two columns for quantity.
        var quantityTypes = cables.Select(idc => idc.Cable.QuantityType).Distinct().ToList();
        var lastQuantityColumn = 7;
        if (quantityTypes.Count == 1)
        {
            _columns.Add(quantityTypes.First(), 7);
            lastQuantityColumn = 7;
        } 
        else if (quantityTypes.Count > 1)
        {
            for (int i = 0; i < quantityTypes.Count; i++)
            {
                _columns.Add(quantityTypes[i], 7 + i);
                lastQuantityColumn = 7 + i;
            }
        }

        _columns.Add("To/From", lastQuantityColumn + 1);
        _columns.Add("Cable Id_", lastQuantityColumn + 2);
        _columns.Add("Keystone", lastQuantityColumn + 3);

        using var workbook = new XLWorkbook(FilePath);

        systemName = RemoveInvalidCharsForExcelSpreadsheetName(systemName);

        var worksheet = workbook.Worksheets.Add(systemName);
        worksheet.SheetView.FreezeRows(1);
        worksheet.Column(3).Width = 30;
        worksheet.Column(4).Width = 30;
        worksheet.Column(7).Width = 5;
        worksheet.Column(10).Width = 5;

        // Write headers
        foreach (var col in _columns)
        {
            worksheet.Cell(1, col.Value).Value = col.Key;
        }
        //worksheet.Cell(1, 1).Value = "Cable Id";
        //worksheet.Cell(1, 2).Value = "Panel Id";
        //worksheet.Cell(1, 3).Value = "Description";
        //worksheet.Cell(1, 4).Value = "Location";
        //worksheet.Cell(1, 5).Value = "Room";
        //worksheet.Cell(1, 6).Value = "AFFL (MM)";
        //worksheet.Cell(1, 7).Value = "Quantity Of Outlets";
        //worksheet.Cell(1, 8).Value = "To/From";
        //worksheet.Cell(1, 9).Value = "Cable Id";
        //worksheet.Cell(1, 10).Value = "Keystone";
        for (int i = 1; i <= _columns.Count; i++)
        {
            worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.AliceBlue;
            worksheet.Cell(1, i).Style.Font.Bold = true;
            worksheet.Cell(1, i).Style.Font.FontSize = 18d;
        }

        // Write data
        int row = 2;
        var keystone = 1;
        var lastCableId = 0;
        var lastDestinationId = "";
        var lastPanelId = "";
        var lastQtyType = "";
        foreach (var data in cables)
        {
            var cable = data.Cable;
            var cableId = data.Id.IdFull;
            var cableIdDigits = data.Id.Digits;
            var cableIdNumber = data.Id.Number;
            var cableIdPrefix = data.Id.Prefix;
            if (cableIdNumber - lastCableId > 1)
            {
                for (int i = lastCableId + 1; i < cableIdNumber; i++)
                {
                    string formatSpecifier = new('0', cableIdDigits);
                    string fNumber = i.ToString(formatSpecifier);
                    var spareId = $"{cableIdPrefix}{fNumber}";

                    worksheet.Cell(row, 1).Value = spareId;
                    worksheet.Cell(row, 2).Value = "";
                    worksheet.Cell(row, 3).Value = "SPARE";
                    worksheet.Cell(row, 4).Value = "";
                    worksheet.Cell(row, 5).Value = "";
                    worksheet.Cell(row, 6).Value = "";
                    lastQuantityColumn = 7;
                    for (int j = 0; j < quantityTypes.Count; j++)
                    {
                        var idx = 7 + j;
                        worksheet.Cell(row, idx).Value = "";
                        lastQuantityColumn = idx;
                    }
                    worksheet.Cell(row, lastQuantityColumn + 1).Value = lastDestinationId;
                    worksheet.Cell(row, lastQuantityColumn + 2).Value = spareId;
                    worksheet.Cell(row, lastQuantityColumn + 3).Value = keystone;
                    FormatRow(worksheet, row, keystone);

                    row++;
                    keystone = IncrementKeystone(keystone);
                    lastCableId = i;
                }
            }

            worksheet.Cell(row, 1).Value = cableId;
            worksheet.Cell(row, 2).Value = cable.PanelId;
            worksheet.Cell(row, 3).Value = cable.Description;
            worksheet.Cell(row, 4).Value = cable.Location;
            worksheet.Cell(row, 5).Value = cable.Room;
            worksheet.Cell(row, 6).Value = cable.AboveFinFloorLvl;
            lastQuantityColumn = 7;
            for (int i = 0; i < quantityTypes.Count; i++)
            {
                var type = quantityTypes[i];
                var idx = 7 + i;
                var qtyCellValue = "";
                if (cable.PanelId != lastPanelId || cable.QuantityType != lastQtyType)
                    qtyCellValue = cable.Quantity + "";

                if (string.Equals(type, cable.QuantityType))
                {
                    worksheet.Cell(row, idx).Value = qtyCellValue;
                }

                lastQuantityColumn = idx;
            }
            worksheet.Cell(row, lastQuantityColumn + 1).Value = cable.DestinationId;
            worksheet.Cell(row, lastQuantityColumn + 2).Value = cableId;
            worksheet.Cell(row, lastQuantityColumn + 3).Value = keystone;
            FormatRow(worksheet, row, keystone, "");

            row++;
            keystone = IncrementKeystone(keystone);
            lastCableId = cableIdNumber;
            lastDestinationId = cable.DestinationId;
            lastPanelId = cable.PanelId;
            lastQtyType = cable.QuantityType;
        }

        // Save the workbook
        workbook.Save();

        _columns.Clear();

        return true;
    }

    public void FormatRow(IXLWorksheet worksheet, int row, int keystone, string qtyCellValue = "")
    {
        var cableIdCell = worksheet.Cell(row, 1);
        var panelIdCell = worksheet.Cell(row, 2);
        var cableIdCell2 = worksheet.Cell(row, 9);
        var rackIdCell = worksheet.Cell(row, 8);
        cableIdCell.Style.Font.Bold = true;
        panelIdCell.Style.Font.Bold = true;
        cableIdCell2.Style.Font.Bold = true;
        rackIdCell.Style.Font.Bold = true;
        cableIdCell.Style.Font.FontSize = 13d;
        panelIdCell.Style.Font.FontSize = 13d;
        cableIdCell2.Style.Font.FontSize = 13d;
        rackIdCell.Style.Font.FontSize = 13d;

        panelIdCell.Style.Font.FontColor = XLColor.GreenPigment;
        rackIdCell.Style.Font.FontColor = XLColor.Blue;

        var roomCell = worksheet.Cell(row, 5);
        roomCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var afflCell = worksheet.Cell(row, 6);
        afflCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var qtyCell = worksheet.Cell(row, 7);
        qtyCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        if (keystone == 24)
        {
            var keystoneCell = worksheet.Cell(row, 10);
            keystoneCell.Style.Fill.BackgroundColor = XLColor.Yellow;
            keystoneCell.Style.Font.Bold = true;
        }

        if (string.IsNullOrEmpty(qtyCellValue))
        {
            qtyCell.Style.Fill.BackgroundColor = XLColor.LightGreen;
        }
    }

    public int IncrementKeystone(int keystone)
    {
        keystone++;
        if (keystone == 25)
        {
            keystone = 1;
        }
        return keystone;
    }

    static string RemoveInvalidCharsForExcelSpreadsheetName(string input)
    {
        string pattern = "[^a-zA-Z0-9]";
        string result = Regex.Replace(input, pattern, "");
        return result.Length > 0 ? (result.Length > 31 ? result.Substring(0, 31) : result) : "Unnamed Sheet";
    }

    static void WriteAndFormatCableIdCell()
    {

    }
}
