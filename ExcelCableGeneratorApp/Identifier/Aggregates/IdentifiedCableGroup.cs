using ExcelCableGeneratorApp.Extract.Aggregates;

namespace ExcelCableGeneratorApp.Identifier.Aggregates;

internal record IdentifiedCableGroup(string Name, List<IdentifiedCable> Cables) { }

internal record IdentifiedCable(StageCraftCableId Id, SystemCableData Cable) {

    public override string ToString()
    {
        return $"{Id.IdFull} ::: {Cable}";
    }
}