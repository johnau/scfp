namespace FaceplateIdGenerator.Aggregates
{
    internal class TechPanelIdentifier : Identifier
    {
        public TechPanelIdentifier(int startNumber = 100) 
            : base("TD", startNumber)
        {
        }
    }
}
