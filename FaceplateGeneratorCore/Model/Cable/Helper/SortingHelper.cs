using System.Diagnostics;
using System.Text.RegularExpressions;
using FaceplateGeneratorCore.Model.Cable;

namespace FaceplateGeneratorCore.Model.Cable.Helper;

internal class SortingHelper
{
    public static List<CableData> GroupBy_System_AndSortBy_Destination_Source_Description_Location_Room(List<CableData> cables)
    {
        var filterDestinationRackId = (CableData cable) =>
        {
            var match = Regex.Match(cable.DestinationRackId, @"\d");
            return cable.DestinationRackId.Substring(match.Index);
        };
        return cables.GroupBy(cable => cable.SystemType.ToString())
                            //.ThenBy(cable => cable.DestinationPanelId)
                            .Select(group => new
                            {
                                Name = group.Key,
                                Cables = group.OrderBy(filterDestinationRackId)
                                                .ThenBy(cable => cable.SourcePanelId)
                                                .ThenBy(cable => cable.Description)
                                                .ThenBy(cable => cable.Location)
                                                .ThenBy(cable => cable.Room)
                            })
                            .SelectMany(a => a.Cables)
                            .ToList();
    }

    // Prety Close
    public static List<CableData> SortBy_Description_AndGroupBy_System_AndSortBy_Source_Destination_Location_Room(List<CableData> cables)
    {
        var filterDestinationRackId = (CableData cable) =>
        {
            var match = Regex.Match(cable.DestinationRackId, @"\d");
            return cable.DestinationRackId.Substring(match.Index);
        };
        return cables.OrderBy(cable => cable.Description)
                        .GroupBy(cable => cable.SystemType.ToString())
                            //.ThenBy(cable => cable.DestinationPanelId)
                            .Select(group => new
                            {
                                Name = group.Key,
                                Cables = group.OrderBy(filterDestinationRackId)
                                                .ThenBy(cable => cable.SourcePanelId)
                                                .ThenBy(cable => cable.Description)
                                                .ThenBy(cable => cable.Location)
                                                .ThenBy(cable => cable.Room)
                            })
                            .SelectMany(a => a.Cables)
                            .ToList();
    }

    // Pretty Close
    public static List<CableData> GroupBy_System_AndSortBy_Description_Destination_Source_Location_Room(List<CableData> cables)
    {
        var filterDestinationRackId = (CableData cable) =>
        {
            var match = Regex.Match(cable.DestinationRackId, @"\d");
            return cable.DestinationRackId.Substring(match.Index);
        };
        return cables.OrderBy(cable => cable.Description)
                        .GroupBy(cable => cable.SystemType.ToString())
                            //.ThenBy(cable => cable.DestinationPanelId)
                            .Select(group => new
                            {
                                Name = group.Key,
                                Cables = group.OrderBy(cable => cable.Description)
                                                .ThenBy(filterDestinationRackId)
                                                .ThenBy(cable => cable.SourcePanelId)
                                                .ThenBy(cable => cable.Location)
                                                .ThenBy(cable => cable.Room)
                            })
                            .SelectMany(a => a.Cables)
                            .ToList();

        throw new Exception("The results of this are far from what we want");
    }

    // Pretty close
    public static List<CableData> GroupBy_System_AndSortBy_Destination_Description_Source_Location_Room(List<CableData> cables)
    {
        var filterDestinationRackId = (CableData cable) =>
        {
            var match = Regex.Match(cable.DestinationRackId, @"\d");
            return cable.DestinationRackId.Substring(match.Index);
        };
        return cables.OrderBy(cable => cable.Description)
                        .GroupBy(cable => cable.SystemType.ToString())
                            //.ThenBy(cable => cable.DestinationPanelId)
                            .Select(group => new
                            {
                                Name = group.Key,
                                Cables = group.OrderBy(filterDestinationRackId)
                                                .ThenBy(cable => cable.Description)
                                                .ThenBy(cable => cable.SourcePanelId)
                                                .ThenBy(cable => cable.Location)
                                                .ThenBy(cable => cable.Room)
                            })
                            .SelectMany(a => a.Cables)
                            .ToList();

    }

    // Pretty bad
    public static List<CableData> GroupBy_System_AndSortBy_Source_Destination_Description_Location_Room(List<CableData> cables)
    {
        var filterDestinationRackId = (CableData cable) =>
        {
            var match = Regex.Match(cable.DestinationRackId, @"\d");
            return cable.DestinationRackId.Substring(match.Index);
        };
        return cables.OrderBy(cable => cable.Description)
                            .GroupBy(cable => cable.SystemType.ToString())
                            //.ThenBy(cable => cable.DestinationPanelId)
                            .Select(group => new
                            {
                                Name = group.Key,
                                Cables = group.OrderBy(cable => cable.SourcePanelId)
                                                    .ThenBy(filterDestinationRackId)
                                                    .ThenBy(cable => cable.Description)
                                                    .ThenBy(cable => cable.Location)
                                                    .ThenBy(cable => cable.Room)
                            })
                            .SelectMany(a => a.Cables)
                            .ToList();

        throw new Exception("This one is pretty far off");

    }

    public static List<CableData> GroupBy_System_AndSortBy_RackQty_AndThenPanelId(List<CableData> cables)
    {
        var sortRack_NumericOnly = (CableData cable) =>
        {
            var match = Regex.Match(cable.DestinationRackId, @"\d");
            if (match.Success)
                return cable.DestinationRackId.Substring(match.Index);
            else
                return cable.DestinationRackId;
        };

        var groupedByRack = cables.GroupBy(sortRack_NumericOnly);
        var orderedRacks = groupedByRack.OrderByDescending(group => group.Count());

        List<string> rackIdPriority = [];
        foreach (var r in orderedRacks)
        {
            foreach (var cable in r)
            {
                var numericIdOnly = sortRack_NumericOnly(cable);
                if (!rackIdPriority.Contains(numericIdOnly))
                {
                    rackIdPriority.Add(numericIdOnly);
                }
            }
        }
        Debug.WriteLine($"{string.Join(", ", rackIdPriority)}");

        return cables.GroupBy(cable => cable.SystemType.ToString())
                            .Select(group => new
                            {
                                Name = group.Key,
                                Cables = group.OrderBy(cable =>
                                {
                                    int index = rackIdPriority.IndexOf(sortRack_NumericOnly(cable));
                                    return index < 0 ? rackIdPriority.Count : index; // index should never be < 0
                                })
                                            .ThenBy(cable => cable.SourcePanelId)
                                            .ToList()
                            })
                            .SelectMany(a => a.Cables)
                            .ToList();
    }

    public static List<CableData> SortSystemGroup(List<CableData> cables)
    {
        var filterRackId_ExcludeLeadingLetters = (CableData cable) =>
        {
            var match = Regex.Match(cable.DestinationRackId, @"\d");
            if (match.Success)
                return cable.DestinationRackId.Substring(match.Index);
            else
                return cable.DestinationRackId;
        };

        var calculateRackPriority = () =>
        {
            var groupedByRack = cables.GroupBy(filterRackId_ExcludeLeadingLetters);
            var orderedRacks = groupedByRack.OrderByDescending(group => group.Count());

            List<string> rackIdPriority = [];
            foreach (var r in orderedRacks)
            {
                foreach (var cable in r)
                {
                    var numericIdOnly = filterRackId_ExcludeLeadingLetters(cable);
                    if (!rackIdPriority.Contains(numericIdOnly))
                    {
                        rackIdPriority.Add(numericIdOnly);
                    }
                }
            }
            return rackIdPriority;
        };

        var rackIdPriority = calculateRackPriority();

        Debug.WriteLine($"{string.Join(", ", rackIdPriority)}");

        var cablesSorted = cables.OrderBy(cable =>
            {
                int index = rackIdPriority.IndexOf(filterRackId_ExcludeLeadingLetters(cable));
                return index < 0 ? rackIdPriority.Count : index; // index should never be < 0
            })
            .ThenBy(cable => cable.SourcePanelId)
            .ToList();

        return cablesSorted;

    }

    // Pretty close
    public static List<CableData> Group_And_Sort(List<CableData> cables)
    {
        var filterDestinationRackId = (CableData cable) =>
        {
            var match = Regex.Match(cable.DestinationRackId, @"\d");
            return cable.DestinationRackId.Substring(match.Index);
        };

        var groupedByDestinationRack = cables.GroupBy(filterDestinationRackId);
        var thenGroupedBySystemAndSorted = groupedByDestinationRack
                .Select(group => new
                {
                    Name = group.Key,
                    Cables = group.GroupBy(cable => cable.SystemType.ToString())
                                    .Select(innerGroup => new
                                    {
                                        Name = innerGroup.Key,
                                        InnerCables = innerGroup.OrderBy(cable => cable.SourcePanelId)
                                                                .ThenBy(cable => cable.Description)
                                                                .ThenBy(cable => cable.Location)
                                                                .ThenBy(cable => cable.Room)
                                    })
                                    .SelectMany(a => a.InnerCables)
                })
                .SelectMany(a => a.Cables)
                .ToList();

        return thenGroupedBySystemAndSorted;

    }
}
