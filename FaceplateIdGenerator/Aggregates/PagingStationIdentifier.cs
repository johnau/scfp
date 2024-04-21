namespace FaceplateIdGenerator.Aggregates
{
    internal class PagingStationIdentifier : Identifier
    {
        public PagingStationIdentifier(int startNumber = 0)
            : base("PS", startNumber)
        {
        }
    }
}
