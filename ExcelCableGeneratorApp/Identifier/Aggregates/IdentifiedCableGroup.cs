namespace ExcelCableGeneratorApp.Identifier.Aggregates;

internal record IdentifiedCableGroup(string Name, List<IdentifiedCable> Cables) {}
