namespace FaceplateIdGenerator.Aggregates
{
    internal class VideoTieLineIdentifier : Identifier
    {
        public VideoTieLineIdentifier(int startNumber = 0) 
            : base("VTL", startNumber)
        {
        }
    }
}
