namespace FaceplateIdGenerator.Aggregates
{
    internal class MultimodeFiberIdentifier : Identifier
    {
        public MultimodeFiberIdentifier(int startNumber = 0) 
            : base("MF", startNumber)
        {
        }
    }
}
