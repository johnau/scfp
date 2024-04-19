using FaceplateDataExtractor.Model;

namespace FaceplateGeneratorCore.Data
{
    public class CableData
    {
        public string Id { get; private set; }
        public SystemType SystemType { get; }
        public string Description { get; }
        public string Location { get; }
        public string Room { get; }
        public string Affl { get; }
        public string CableType { get; }
        public string SourcePanelId { get; }
        public string DestinationPanelId { get; }

        public CableData(SystemType systemType, string id, string description, string location, string room, string affl, string cableType, string sourcePanelId, string destinationPanelId)
        {

            Id = id;
            SystemType = systemType;
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
            string truncatedDescription = Description.Length > 25 ? Description[..25] : Description;
            string truncatedLocation = Location.Length > 25 ? Location[..25] : Location;
            string truncatedRoom = Room.Length > 10 ? Room[..10] : Room;
            string truncatedAffl = Affl.Length > 10 ? Affl[..10] : Affl;
            string truncatedSourcePanelId = SourcePanelId.Length > 10 ? SourcePanelId[..10] : SourcePanelId;
            string truncatedDestinationPanelId = DestinationPanelId.Length > 10 ? DestinationPanelId[..10] : DestinationPanelId;
            string truncatedCableType = CableType.Length > 10 ? CableType[..10] : CableType;

            return $"{"Cable:",-10} {Id,-10} D: {truncatedDescription,-25} L: {truncatedLocation,-25} R: {truncatedRoom,-10} A: {truncatedAffl,-10} FROM: {truncatedSourcePanelId,-10} TO: {truncatedDestinationPanelId,-10} C: {truncatedCableType,-10} SYS: {SystemType, -10}";
        }

        public void AssignId(string id)
        {
            Id = id;
        }

    }
}
