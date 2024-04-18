namespace FaceplateIdGenerator.Aggregates
{
    internal class VideoTieLineIdentifier : Identifier
    {
        public VideoTieLineIdentifier(int startNumber = 100) 
            : base("VTL", startNumber)
        {
        }
    }
}
