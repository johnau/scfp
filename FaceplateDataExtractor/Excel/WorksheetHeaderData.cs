using System.Diagnostics;
using System.Text;
using FaceplateDataExtractor.Model;

namespace FaceplateDataExtractor.Excel
{
    internal class WorksheetHeaderData
    {
        public string FilePath { get; }
        public int SheetNumber { get; }
        public int PanelIdColumn { get; set; }
        public int DescriptionColumn{ get; set; }
        public int LocationColumn { get; set; }
        public int RoomColumn { get; set; }
        public int AboveFinishedFloorLevelColumn { get; set; }
        public Dictionary<int, List<string>> CableColumns { get; }

        public WorksheetHeaderData(string filePath, int sheet = 1)
        {
            FilePath = filePath;
            SheetNumber = sheet;
            PanelIdColumn = -1;
            DescriptionColumn = -1;
            LocationColumn = -1;
            RoomColumn = -1;
            AboveFinishedFloorLevelColumn = -1;
            CableColumns = [];
        }

        /// <summary>
        /// Add a Cable Column header
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="headerTexts"></param>
        /// <exception cref="Exception">Throws exception on duplicate Key</exception>
        public void AddCableColumn(int columnIndex, List<string> headerTexts)
        {
            if (!CableColumns.TryAdd(columnIndex, headerTexts))
            {
                throw new Exception("Unable to add a duplicated column header");
            }
        }

        public List<string>? GetCableColumnValues(int colIndex)
        {
            if (CableColumns.TryGetValue(colIndex, out var values))
            {
                return values;
            } else
            {
                return null;
            }
        }

        public List<string> GetHeadersByColumnIndex(int columnIndex)
        {
            switch (columnIndex)
            {
                case 1:
                    return ["PANEL ID"];
                case 2:
                    return ["DESCRIPTION"];
                case 3:
                    return ["LOCATION"];
                case 4:
                    return ["ROOM"];
                case 5:
                    return ["AFFL"];
                default:
                    break;
            }

            if (!CableColumns.TryGetValue(columnIndex, out var values))
            {
                throw new ArgumentException($"The column at: {columnIndex}, does not have a header value or was not found");
            }

            return values;
            //var sb = new StringBuilder();
            //for (int i = 0; i < values.Count; i++)
            //{
            //    var v = values[i];
            //    if (v == "") continue;

            //    sb.Append('{');
            //    sb.Append(v);
            //    sb.Append('}');
            //    if (i != values.Count - 1) sb.Append(", ");
            //}
            //return sb.ToString();
        }

        public void SetExpectedHeaderColumn(PanelDescriptorDataType expHeader, int columnIndex)
        {
            switch (expHeader)
            {
                case PanelDescriptorDataType.NONE:
                    Debug.WriteLine($"Received a NONE type Data Column Type");
                    break;
                case PanelDescriptorDataType.PANEL_ID:
                    PanelIdColumn = columnIndex;
                    break;
                case PanelDescriptorDataType.DESCRIPTION:
                    DescriptionColumn = columnIndex;
                    break;
                case PanelDescriptorDataType.LOCATION:
                    LocationColumn = columnIndex;
                    break;
                case PanelDescriptorDataType.ROOM:
                    RoomColumn = columnIndex;
                    break;
                case PanelDescriptorDataType.AFFL:
                    AboveFinishedFloorLevelColumn = columnIndex;
                    break;
                default:
                    throw new ArgumentException($"Unrecognized Expected Header Value: {expHeader.ToString()}");
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var header in CableColumns)
            {
                sb.Append("Cable: ");
                foreach (var s in header.Value)
                {
                    sb.Append(s);
                    sb.Append(", ");
                }
                sb.Append('=');
                sb.Append(header.Key);
                sb.Append(", \n");
            }
            return $"WorksheetHeaderData[\n" +
                $"panel id='{PanelIdColumn}', \n" +
                $"description='{DescriptionColumn}', \n" +
                $"location='{LocationColumn}', \n" +
                $"room='{RoomColumn}', \n" +
                $"affl='{AboveFinishedFloorLevelColumn}', \n" +
                sb.ToString() +
                $"]";
        }
    }
}
