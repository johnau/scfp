namespace FaceplateIdGenerator.Aggregates
{
    internal class AudioIdentifier : Identifier
    {
        public AudioIdentifier(int startNumber = 0) 
            : base("A", startNumber)
        {
        }
    }
}
