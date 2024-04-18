namespace FaceplateGeneratorCore.Data
{
    public class CableData
    {
        public string Id { get; }
        public string Description { get; }
        public string Location { get; }
        public string Room { get; }
        public string Affl { get; }
        public string CableType { get; }
        public string SourcePanelId { get; }
        public string DestinationPanelId { get; }

        public CableData(string id, string description, string location, string room, string affl, string cableType, string sourcePanelId, string destinationPanelId)
        {
            Id = id;
            Description = description;
            Room = room;
            Location = location;
            CableType = cableType;
            Affl = affl;
            SourcePanelId = sourcePanelId;
            DestinationPanelId = destinationPanelId;
        }

        public override string ToString()
        {
            string truncatedLocation = Location.Length > 25 ? Location.Substring(0, 25) : Location;
            string truncatedRoom = Room.Length > 10 ? Room.Substring(0, 10) : Room;
            string truncatedAffl = Affl.Length > 10 ? Affl.Substring(0, 10) : Affl;
            string truncatedSourcePanelId = SourcePanelId.Length > 10 ? SourcePanelId.Substring(0, 10) : SourcePanelId;
            string truncatedDestinationPanelId = DestinationPanelId.Length > 10 ? DestinationPanelId.Substring(0, 10) : DestinationPanelId;
            string truncatedCableType = CableType.Length > 10 ? CableType.Substring(0, 10) : CableType;

            return $"{"Cable:",-10} {Id,-10} L: {truncatedLocation,-25} R: {truncatedRoom,-10} A: {truncatedAffl,-10} S: {truncatedSourcePanelId,-10} D: {truncatedDestinationPanelId,-10} C: {truncatedCableType,-10}";
        }

    }
}
