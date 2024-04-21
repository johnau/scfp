namespace FaceplateIdGenerator.Aggregates
{
    internal class TalkbackIdentifier : Identifier
    {
        public TalkbackIdentifier(int startNumber = 0)
            : base("TB", startNumber)
        {
        }
    }
}
