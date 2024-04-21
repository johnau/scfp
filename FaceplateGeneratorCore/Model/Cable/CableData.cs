using FaceplateDataExtractor.Model;
using System.Diagnostics;

namespace FaceplateGeneratorCore.Model.Cable
{
    public class CableData
    {
        public static CableData PLACEHOLDER(string id)
        {
            return new CableData(SystemType.NONE, id, "SPARE", "", "", "", "", "", "", -1);
        }

        public string Id { get; private set; }
        public int IdNumberOnly => GetNumberFromId();
        public SystemType SystemType { get; }
        public string Description { get; }
        public string Location { get; }
        public string Room { get; }
        public string Affl { get; }
        public string CableType { get; }
        public string SourcePanelId { get; }
        public string DestinationRackId { get; }
        public int InGroupOfXCables { get; }
        public int Keystone => IdToKeystone();

        public CableData(SystemType systemType, string id, string description, string location, string room, string affl, string cableType, string sourcePanelId, string destinationPanelId, int inGroupOfXCables)
        {

            Id = id;
            SystemType = systemType;
            Description = description;
            Room = room;
            Location = location;
            CableType = cableType;
            Affl = affl;
            SourcePanelId = sourcePanelId;
            DestinationRackId = destinationPanelId;
            InGroupOfXCables = inGroupOfXCables;
        }

        public override string ToString()
        {
            string truncatedDescription = Description.Length > 25 ? Description[..25] : Description;
            string truncatedLocation = Location.Length > 25 ? Location[..25] : Location;
            string truncatedRoom = Room.Length > 10 ? Room[..10] : Room;
            string truncatedAffl = Affl.Length > 10 ? Affl[..10] : Affl;
            string truncatedSourcePanelId = SourcePanelId.Length > 10 ? SourcePanelId[..10] : SourcePanelId;
            string truncatedDestinationRackId = DestinationRackId.Length > 10 ? DestinationRackId[..10] : DestinationRackId;
            string truncatedCableType = CableType.Length > 10 ? CableType[..10] : CableType;

            return $"{"Cable:",-10} {Id,-10} D: {truncatedDescription,-25} L: {truncatedLocation,-25} R: {truncatedRoom,-10} A: {truncatedAffl,-10} FROM: {truncatedSourcePanelId,-10} TO: {truncatedDestinationRackId,-10} SYS: {SystemType,-10} C: {truncatedCableType,-10}";
        }
        public string ToCsvString()
        {
            return $"{Id},{SourcePanelId},{Description},{Location},{Room},{Affl},{DestinationRackId},{Id},{InGroupOfXCables},{Keystone}";
        }

        public void AssignId(string id)
        {
            Id = id;
        }

        private int GetNumberFromId()
        {
            var digitIndex = FindFirstDigitIndex(Id);
            if (digitIndex == -1)
                return -1;

            var digitStr = Id[digitIndex..];
            if (int.TryParse(digitStr, out var intValue))
            {
                Debug.WriteLine($"Id: {Id} -> Number only: {intValue}");
                return intValue;
            }

            return -1;
        }

        private int IdToKeystone()
        {
            var intValue = GetNumberFromId();
            if (intValue != -1)
            {
                var keystone = intValue % 24;
                if (keystone == 0) 
                    keystone = 24;

                return keystone;
            }

            return -1;
        }

        private static int FindFirstDigitIndex(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    return i;
                }
            }
            // If no digit is found, return -1 or throw an exception, depending on your requirements.
            return -1;
        }

    }
}
