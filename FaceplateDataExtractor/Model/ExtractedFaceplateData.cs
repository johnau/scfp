namespace FaceplateDataExtractor.Model
{
    /// <summary>
    /// Represents one entry of faceplate data extracted from a data source.
    /// </summary>
    public class ExtractedFaceplateData
    {
        public string PanelId { get; }
        public string Description { get; }
        public string Location { get; }
        public string Room { get; }
        public string AboveFinishedFloorLevel { get; }
        private List<CableSystemData> _cableSystemDatas;
        public List<CableSystemData> CableSystemDatas => new List<CableSystemData>(_cableSystemDatas);

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
    }
}
