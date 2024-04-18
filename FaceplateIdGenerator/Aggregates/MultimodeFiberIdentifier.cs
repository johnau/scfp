namespace FaceplateIdGenerator.Aggregates
{
    internal class MultimodeFiberIdentifier : Identifier
    {
        public MultimodeFiberIdentifier(int startNumber = 100) 
            : base("MF", startNumber)
        {
        }
    }
}
