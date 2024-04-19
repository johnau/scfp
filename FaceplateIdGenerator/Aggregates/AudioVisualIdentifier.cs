namespace FaceplateIdGenerator.Aggregates
{
    internal class AudioVisualIdentifier : Identifier
    {
        public AudioVisualIdentifier(int startNumber = 0) 
            : base("AV", startNumber)
        {
        }
    }
}
