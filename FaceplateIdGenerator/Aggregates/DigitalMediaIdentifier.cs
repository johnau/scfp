namespace FaceplateIdGenerator.Aggregates
{
    internal class DigitalMediaIdentifier : Identifier
    {
        public DigitalMediaIdentifier(int startNumber = 0) 
            : base("DM", startNumber)
        {
        }
    }
}
