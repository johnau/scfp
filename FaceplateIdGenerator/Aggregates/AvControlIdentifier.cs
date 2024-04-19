namespace FaceplateIdGenerator.Aggregates
{
    internal class AvControlIdentifier : Identifier
    {
        public AvControlIdentifier(int startNumber = 0) 
            : base("AVC", startNumber)
        {
        }
    }
}
