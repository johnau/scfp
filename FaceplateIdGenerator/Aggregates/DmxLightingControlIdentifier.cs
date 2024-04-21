namespace FaceplateIdGenerator.Aggregates;

internal class DmxLightingControlIdentifier : Identifier
{
    public DmxLightingControlIdentifier(int startNumber = 0)
        : base("DMX", startNumber)
    {
    }
}
