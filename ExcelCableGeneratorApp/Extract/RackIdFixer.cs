using ExcelCableGeneratorApp.Extract.Aggregates;

namespace ExcelCableGeneratorApp.Extract;

/// <summary>
/// Analyzers Rack Ids and provides functionality to fix minor issues (i.e. typos)
/// </summary>
internal class RackIdFixer
{
    private readonly List<SystemCableData> Cables;
    private readonly Dictionary<string, string> NameSubs;

    public RackIdFixer(List<SystemCableData> cables)
    {
        Cables = cables;
        NameSubs = [];
    }

    public List<string> GetListOfUniqueRackNames()
    {
        return Cables.Select(cable => cable.DestinationId).Distinct().ToList();
    }

    public void AddNameSubstition(string nameKeep, string nameDrop)
    {
        NameSubs.Add(nameDrop, nameKeep);
    }

    public string SubstituteNameIfRequired(string name)
    {
        if (NameSubs.TryGetValue(name, out var substitute))
        {
            return substitute;
        }

        return name;
    }
}
