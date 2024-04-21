using ExcelCableGeneratorApp.Extract.Aggregates;

namespace ExcelCableGeneratorApp.Extract
{
    internal class SimpleExcelDataExtractor : IExcelDataExtractor
    {
        public bool TryExtractData(out List<SystemCableData> data, out List<SystemCableData> rejectedData)
        {
            throw new NotImplementedException();
        }
    }
}