namespace FaceplateIdGenerator.Aggregates;

internal class PerformanceLoudSpeakerIdentifier : Identifier
{
    public PerformanceLoudSpeakerIdentifier(int startNumber = 0)
        : base("PA", startNumber)
    {
    }
}
