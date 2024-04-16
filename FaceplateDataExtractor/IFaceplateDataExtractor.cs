using FaceplateDataExtractor.Model;

namespace FaceplateDataExtractor
{
    public interface IFaceplateDataExtractor
    {
        bool TryExtractData(string filePath, out List<ExtractedFaceplateData> data);
        List<string> GetErrors();
    }
}
