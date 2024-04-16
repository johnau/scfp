using FaceplateDataExtractor.Model;

namespace FaceplateDataExtractor.Excel
{
    public class MsExcelFaceplateDataExtractor : IFaceplateDataExtractor
    {
        public List<string> GetErrors()
        {
            throw new NotImplementedException();
        }

        public bool TryExtractData(string filePath, out List<ExtractedFaceplateData> data)
        {
            throw new NotImplementedException();
        }
    }
}
