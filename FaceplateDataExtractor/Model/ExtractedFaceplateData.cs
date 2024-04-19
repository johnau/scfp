using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FaceplateDataExtractor.Utility;
using System.Diagnostics;

namespace FaceplateDataExtractor.Model
{
    /// <summary>
    /// Represents one entry of faceplate data extracted from a data source.
    /// </summary>
    public class ExtractedFaceplateData
    {
        /// <summary>
        /// Source Panel
        /// </summary>
        public string PanelId { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Room { get; set; }
        public string AboveFinishedFloorLevel { get; set; }
        private List<CableSystemData> _cableSystemDatas;
        public List<CableSystemData> CableSystemDatas => new List<CableSystemData>(_cableSystemDatas);

        public ExtractedFaceplateData()
        {
            PanelId = "";
            Description = "";
            Location = "";
            Room = "";
            AboveFinishedFloorLevel = "";
            _cableSystemDatas = [];
        }

        public ExtractedFaceplateData(string panelId, 
                                        string description, 
                                        string location, 
                                        string room, 
                                        string aboveFinishedFloorLevel,
                                        List<CableSystemData> cableSystemDatas)
        {
            PanelId = panelId;
            Description = description;
            Location = location;
            Room = room;
            AboveFinishedFloorLevel = aboveFinishedFloorLevel;
            _cableSystemDatas = cableSystemDatas;
        }

        public void SetSantiziedPanelId(string panelId) => PanelId = StringsHelper.Sanitize(panelId);
        public void SetSantiziedDescription(string description) => Description = StringsHelper.Sanitize(description);
        public void SetSantiziedLocation(string location) => Location = StringsHelper.Sanitize(location);
        public void SetSantiziedRoom(string room) => Room = StringsHelper.Sanitize(room);
        public void SetSantiziedAboveFinishedFloorLevel(string affl) => AboveFinishedFloorLevel = StringsHelper.Sanitize(affl);

        public void AddCableSystem(CableSystemData cableSystemData)
        {
            if (cableSystemData.SystemType != SystemType.NONE 
                && cableSystemData.Quantity > 0 
                && !string.IsNullOrEmpty(cableSystemData.DestPanelId))
                _cableSystemDatas.Add(cableSystemData);
            Debug.WriteLine($"Added cable system data, now have: {_cableSystemDatas.Count} in list");
        }

        public override string ToString()
        {
            return $"ExtractedFaceplateData[\n" +
                    $"PanelId={PanelId}, " +
                    $"Description={Description}, " +
                    $"Location={Location}, " +
                    $"Room={Room}, " +
                    $"AboveFinishedFloorLevel={AboveFinishedFloorLevel}, " +
                    $"# of CableSystems={_cableSystemDatas.Count}" +
                    $"\n]";
        }
    }
}
