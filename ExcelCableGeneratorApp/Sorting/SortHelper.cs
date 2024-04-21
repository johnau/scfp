using ExcelCableGeneratorApp.Extract.Aggregates;
using ExcelCableGeneratorApp.Sorting.Aggregates;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ExcelCableGeneratorApp.Sorting
{
    internal partial class SortHelper
    {

        public static List<SystemCableGroup> GroupBySystem(List<SystemCableData> cables)
        {
            var groups = cables.GroupBy(cable => cable.SystemType)
                    .Select(group => new SystemCableGroup(group.Key, group.ToList()))
                    .ToList();

            return groups;
        }

        public static List<SystemCableData> SortSystemGroup(List<SystemCableData> cables)
        {
            var filterRackId_ExcludeLeadingLetters = (SystemCableData cable) =>
            {
                var match = Regex.Match(cable.DestinationId, @"\d");
                if (match.Success)
                    return cable.DestinationId.Substring(match.Index);
                else
                    return cable.DestinationId;
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
                .ThenBy(cable => cable.PanelId)
                .ToList();

            return cablesSorted;

        }
    }
}
