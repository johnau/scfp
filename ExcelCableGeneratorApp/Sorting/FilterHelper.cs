using ExcelCableGeneratorApp.Dxf.Aggregates.Data;

namespace ExcelCableGeneratorApp.Sorting;

internal class FilterHelper
{

    public FilterHelper()
    {
    }

    public static List<SystemGroupContents> FilterSystemGroupContentsForTechPanels(List<SystemGroupContents> groupContents)
    {
        List<string> desiredSystems = [
            "technical data",
            "digital media",
            "av control data",
            "audio",
            "dante",
            "talkback",
            "performance relay input",
            "paging station",
            "paging volume",
            "paging speaker",
            "dmx",
            "stage lighting",
        ];

        List<string> undesiredSystems = [
            "gpo double outlet",
            "audio power double outlet",
            "3 phase outlet",
            "work light outlet",
            "outlets single 10a",
            "multimode fiber"
        ];

        var filtered = groupContents.Where(group =>
        {
            for (int i = 0; i < desiredSystems.Count; i++)
            {
                if (group.SystemName.Contains(desiredSystems[i], StringComparison.CurrentCultureIgnoreCase))
                {
                    var isNotInUndesiredList = true;
                    for (int i2 = 0; i2 < undesiredSystems.Count; i2++)
                    {
                        if (group.SystemName.Contains(undesiredSystems[i2], StringComparison.CurrentCultureIgnoreCase))
                            isNotInUndesiredList = false;
                    }
                    if (isNotInUndesiredList)
                        return true;
                }
            }
            return false;
        })
        .ToList();

        return filtered;
    }
}
