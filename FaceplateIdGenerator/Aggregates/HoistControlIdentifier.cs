namespace FaceplateIdGenerator.Aggregates;

internal class HoistControlIdentifier : Identifier
{
    public HoistControlIdentifier(int startNumber = 0)
        : base("MX", startNumber)
    {
    }
}
