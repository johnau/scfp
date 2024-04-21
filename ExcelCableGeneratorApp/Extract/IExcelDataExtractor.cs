using ExcelCableGeneratorApp.Extract.Aggregates;

namespace ExcelCableGeneratorApp.Extract;

public interface IExcelDataExtractor
{

    public bool TryExtractData(out List<SystemCableData> data, out List<SystemCableData> rejectedData);
}
