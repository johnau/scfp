namespace FaceplateIdGenerator.Aggregates
{
    internal class AudioIdentifier : Identifier
    {
        public AudioIdentifier(int startNumber = 100) 
            : base("A", startNumber)
        {
        }
    }
}
