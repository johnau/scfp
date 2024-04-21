namespace FaceplateIdGenerator.Aggregates
{
    internal class PagingSpeakerIdentifier : Identifier
    {
        public PagingSpeakerIdentifier(int startNumber = 0)
            : base("PSP", startNumber)
        {
        }
    }
}
