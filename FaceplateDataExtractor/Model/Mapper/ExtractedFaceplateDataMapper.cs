using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FaceplateDataExtractor.Excel;
using FaceplateDataExtractor.Utility;
using Microsoft.VisualBasic;

namespace FaceplateDataExtractor.Model.Mapper
{
    internal class ExtractedFaceplateDataMapper
    {

        public ExtractedFaceplateData Map(WorksheetRowData rowDat)
        {
            var data = rowDat.RowData;
            string panelId = "";
            string description = "";
            string location = "";
            string room = "";
            string aboveFinishedFloorLevel = "";
            List<CableSystemData> cableSystemDatas = [];
                        
            foreach (var cell in data)
            {
                
                var headerTextGroup = cell.Key;
                var cellData = cell.Value;

                // process header text to get Sysetm Type and Cable Type

                var systemType = SystemType.NONE;
                var cableType = CableType.NONE;
                var quantity = 0;
                var destination = "UNKNOWN";

                foreach (SystemType sysType in Enum.GetValues(typeof(SystemType)))
                {
                    var parts = sysType.GetStringArrayValue();
                    cellData.HeaderText.ToLower().Contains("");
                }

                var cableSystemData = new CableSystemData(systemType, cableType, quantity, destination);
            }

            return new ExtractedFaceplateData(panelId, description, location, room, aboveFinishedFloorLevel, cableSystemDatas);
        }


    }
}
