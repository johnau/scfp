namespace FaceplateIdGenerator.Aggregates
{
    internal class StageLightingOutletsIdentifier : Identifier
    {
        public StageLightingOutletsIdentifier(int startNumber = 0)
            : base("SLO", startNumber)
        {
        }
    }
}
