namespace FaceplateIdGenerator.Aggregates
{
    internal class DigitalMediaIdentifier : Identifier
    {
        public DigitalMediaIdentifier(int startNumber = 100) 
            : base("DM", startNumber)
        {
        }
    }
}
