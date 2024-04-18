namespace FaceplateIdGenerator.Aggregates
{
    internal class AvControlIdentifier : Identifier
    {
        public AvControlIdentifier(int startNumber = 100) 
            : base("AVC", startNumber)
        {
        }
    }
}
