using ClosedXML.Excel;
using FaceplateDataExtractor.Model;
using FaceplateDataExtractor.Model.Mapper;
using FaceplateDataExtractor.Utility;
using System.Diagnostics;

namespace FaceplateDataExtractor.Excel
{
    public class ControlledMsExcelFaceplateDataExtractor : IFaceplateDataExtractor
    {
        private readonly Configuration _configuration;
        private string _filePath;
        private int _sheet;

        public record TableLayout(int HeaderStartRow, int HeaderEndRow, int DataStartRow, int DataEndRow);
        public record ColumnLayout(string Name, int ColumnNumber, int HeaderStartRow, int HeaderEndRow);
        public record ColumnSet(string Name, List<ColumnLayout> ColumnsInGroup);
        public record Configuration(int DataStartRow, int DataEndRow, Dictionary<string, ColumnLayout> Columns, Dictionary<string, ColumnSet> ColumnGroups);

        public ControlledMsExcelFaceplateDataExtractor(string filePath, int sheet, Configuration configuration)
        {
            _filePath = filePath;
            _sheet = sheet;
            _configuration = configuration;
        }

        public bool HasErrors => throw new NotImplementedException();

        public List<string> Errors => throw new NotImplementedException();

        private static readonly char[] separator = new char[] { ' ', '\n', '\r' };

        private string GetCellValueAsString(IXLCell cell)
        {
            if (cell == null) return "";

            if (cell.Value.IsBlank) return "";
            if (cell.Value.IsNumber) return cell.GetDouble() + "";
            if (cell.Value.IsText) return cell.GetText();

            Debug.WriteLine($"Unhandled cell type, return empty");

            return "";
        }

        public bool TryExtractData(int flag, out List<ExtractedFaceplateData> data, out List<ExtractedFaceplateData> rejectedData)
        {
            data = [];
            rejectedData = [];

            var workbook = new XLWorkbook(_filePath);
            var worksheet = workbook.Worksheet(_sheet);
            var rows = worksheet.Rows();
            var ixlColumns = worksheet.Columns();

            var columnMappings = _configuration.Columns.Values
                .SelectMany(layout => ixlColumns
                    .Where(column => column.ColumnNumber() == layout.ColumnNumber)
                    .Select(column => new { Layout = layout, Column = column }))
                    .ToDictionary(pair => pair.Layout, pair => pair.Column);

            foreach (var columnMapping in columnMappings)
            {
                var xlsColumn = columnMapping.Value;
                columnMapping.Key.Deconstruct(out var name,
                                                out var columnNumber,
                                                out var headerStartRow,
                                                out var headerEndRow);

                // Accumulate header data for this column
                List<IXLCell> headerCells = [];
                for (int i = headerStartRow; i <= headerEndRow; i++)
                {
                    headerCells.Add(xlsColumn.Cell(i));
                }
            }

            var dataRows = worksheet.Rows(_configuration.DataStartRow, _configuration.DataEndRow);

            var singleColumnDescriptors = _configuration.Columns;
            var groupColumnDescriptors = _configuration.ColumnGroups;
            // looping over the rows
            foreach (var row in dataRows)
            {
                var model = new ExtractedFaceplateData();
                model.SetSantiziedPanelId(GetCellValueAsString(row.Cell(singleColumnDescriptors[PanelDescriptorDataType.PANEL_ID.ToString()].ColumnNumber)));
                model.SetSantiziedDescription(GetCellValueAsString(row.Cell(singleColumnDescriptors[PanelDescriptorDataType.DESCRIPTION.ToString()].ColumnNumber)));
                model.SetSantiziedLocation(GetCellValueAsString(row.Cell(singleColumnDescriptors[PanelDescriptorDataType.LOCATION.ToString()].ColumnNumber)));
                model.SetSantiziedRoom(GetCellValueAsString(row.Cell(singleColumnDescriptors[PanelDescriptorDataType.ROOM.ToString()].ColumnNumber)));
                model.SetSantiziedAboveFinishedFloorLevel(GetCellValueAsString(row.Cell(singleColumnDescriptors[PanelDescriptorDataType.AFFL.ToString()].ColumnNumber)));

                // looping over the descriptors for group columns
                foreach (var kvp in groupColumnDescriptors)
                {
                    var systemData = new CableSystemData();
                    var header = kvp.Key;
                    var groupColumnsDescriptor = kvp.Value.ColumnsInGroup;

                    foreach (var col in groupColumnsDescriptor)
                    {
                        col.Deconstruct(out var name, out var colNum, out var headerStart, out var headerEnd);

                        systemData.SystemType = SystemType.NONE;
                        ExtractedFaceplateDataMapper.ContainsMatchingType<SystemType>(header, out var sysType);
                        systemData.SystemType = sysType;

                        systemData.CableType = CableType.NONE;

                        if (name == ColumnValueType.QUANTITY.ToString() 
                            || name == ColumnValueType.QUANTITY_MALE.ToString()
                            || name == ColumnValueType.QUANTITY_FEMALE.ToString())
                        {
                            var cellValue = GetCellValueAsString(row.Cell(colNum));
                            if (cellValue == null || cellValue == "")
                                continue;

                            var cellValueStart = cellValue.Split(separator, StringSplitOptions.RemoveEmptyEntries)[0];

                            // Set Quantity
                            if (int.TryParse(cellValue, out var result))
                                systemData.Quantity = result;
                            else if (int.TryParse(cellValueStart, out result)) // some cells have 
                                systemData.Quantity = result;
                        }
                        else if (name == ColumnValueType.TO_FROM.ToString())
                        {
                            // Set Destination / TO/FROM
                            systemData.Destination = GetCellValueAsString(row.Cell(colNum));
                        }
                        //else if (name == ColumnValueType.QUANTITY_MALE.ToString())
                        //{

                        //}
                        //else if (name == ColumnValueType.QUANTITY_FEMALE.ToString())
                        //{

                        //}
                        else
                        {
                            throw new Exception("Unhandled Column in ColumnGroup");
                        }
                    }

                    model.AddCableSystem(systemData);
                }

                data.Add(model);
            }

            data = data
                .Where(data => !string.IsNullOrEmpty(data.Description) && !string.IsNullOrEmpty(data.Location)) // Filter out entries with empty Description or Location
                .Where(data => data.CableSystemDatas.Any()) // Filter out entries with empty CableSystemDatas list
                .ToList();

            return true;
        }


    }
}
