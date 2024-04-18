namespace FaceplateIdGenerator.Aggregates
{
    internal class AudioVisualIdentifier : Identifier
    {
        public AudioVisualIdentifier(int startNumber = 100) 
            : base("AV", startNumber)
        {
        }
    }
}
