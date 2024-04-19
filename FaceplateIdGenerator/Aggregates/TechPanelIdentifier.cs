namespace FaceplateIdGenerator.Aggregates
{
    internal class TechPanelIdentifier : Identifier
    {
        public TechPanelIdentifier(int startNumber = 0) 
            : base("TD", startNumber)
        {
        }
    }
}
