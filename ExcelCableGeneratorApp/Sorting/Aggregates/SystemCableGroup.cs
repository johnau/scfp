using ExcelCableGeneratorApp.Extract.Aggregates;

namespace ExcelCableGeneratorApp.Sorting.Aggregates;

public record SystemCableGroup(string Name, List<SystemCableData> Cables);
