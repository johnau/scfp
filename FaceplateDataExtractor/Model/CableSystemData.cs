namespace FaceplateDataExtractor.Model
{
    public class CableSystemData
    {
        public SystemType SystemType { get; }
        public CableType CableType { get; }
        public int Quantity { get; }
        public string Destination { get; }

        public CableSystemData(SystemType systemType, CableType cableType, int quantity, string destination)
        {
            SystemType = systemType;
            CableType = cableType;
            Quantity = quantity;
            Destination = destination;
        }
    }
}
