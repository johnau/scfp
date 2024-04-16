using ClosedXML.Excel;
using FaceplateDataExtractor.Model;

namespace FaceplateDataExtractor.Excel
{
    public class MsExcelFaceplateDataExtractor : IFaceplateDataExtractor
    {
        public List<string> GetErrors()
        {
            throw new NotImplementedException();
        }

        public bool TryExtractData(string filePath, int sheet = 0, out List<ExtractedFaceplateData> data)
        {
            data = [];

            var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(sheet);

            var rows = worksheet.Rows();
            


            return true;
        }
    }
}
