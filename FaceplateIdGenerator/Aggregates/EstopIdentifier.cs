namespace FaceplateIdGenerator.Aggregates
{
    internal class EstopIdentifier : Identifier
    {
        public EstopIdentifier(int startNumber = 0)
            : base("ES", startNumber)
        {
        }
    }
}
