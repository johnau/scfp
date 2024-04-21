namespace FaceplateIdGenerator.Aggregates
{
    internal class TechDataIdentifier : Identifier
    {
        public TechDataIdentifier(int startNumber = 0) 
            : base("TD", startNumber)
        {
        }
    }
}
