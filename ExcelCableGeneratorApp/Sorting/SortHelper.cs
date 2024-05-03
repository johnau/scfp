using ExcelCableGeneratorApp.Extract.Aggregates;
using ExcelCableGeneratorApp.Identifier.Aggregates;
using ExcelCableGeneratorApp.Sorting.Aggregates;
using System.Diagnostics;

namespace ExcelCableGeneratorApp.Sorting
{
    internal partial class SortHelper
    {

        public static List<SystemCableGroup> GroupBySystem(List<SystemCableData> cables)
        {
            var groups = cables.GroupBy(cable => cable.SystemType)
                                .Select(group => new SystemCableGroup(group.Key, [.. group]))
                                .ToList();

            return groups;
        }

        public static List<SystemCableGroup> GroupByLocation(List<SystemCableData> cables) 
        {
            var groups = cables.GroupBy(cable => cable.Location)
                    .Select(group => new SystemCableGroup(group.Key, [.. group]))
                    .ToList();

            return groups;
        }

        public static List<IdentifiedCableGroup> GroupByDest(List<IdentifiedCable> cables)
        {
            var groups = cables.GroupBy(idCable => idCable.Cable.DestinationId)
                                .Select(group => new IdentifiedCableGroup(group.Key + "", [..group]))
                                .ToList();

            return groups;
        }

        public static List<IdentifiedCableGroup> GroupBySource(List<IdentifiedCable> cables)
        {
            var groups = cables.GroupBy(idCable => idCable.Cable.PanelId)
                                .Select(group => new IdentifiedCableGroup(group.Key + "", [.. group]))
                                .ToList();

            return groups;
        }

        public static List<IdentifiedCableGroup> GroupIdentifiedBySystem(List<IdentifiedCable> cables)
        {
            var groups = cables.GroupBy(idCable => idCable.Cable.SystemType)
                                .Select(group => new IdentifiedCableGroup(group.Key + "", [.. group]))
                                .ToList();

            return groups;
        }

        public static List<SystemCableData> SortSystemGroup(List<SystemCableData> cables)
        {
            // Not sorting by the numbers only anymore, as this will collide with other rack id's.  ie. There can be ER601 and MX601
            //var filterRackId_ExcludeLeadingLetters = (SystemCableData cable) =>
            //{
            //    var match = Regex.Match(cable.DestinationId, @"\d");
            //    if (match.Success)
            //        return cable.DestinationId.Substring(match.Index);
            //    else
            //        return cable.DestinationId;
            //};

            var calculateRackPriority = () =>
            {
                //var groupedByRack = cables.GroupBy(filterRackId_ExcludeLeadingLetters);
                var groupedByRack = cables.GroupBy(c => c.DestinationId);
                var orderedRacks = groupedByRack.OrderByDescending(group => group.Count());

                List<string> rackIdPriority = [];
                foreach (var r in orderedRacks)
                {
                    foreach (var cable in r)
                    {
                        //var numericIdOnly = filterRackId_ExcludeLeadingLetters(cable);
                        //if (!rackIdPriority.Contains(numericIdOnly))
                        if (!rackIdPriority.Contains(cable.DestinationId))
                        {
                            rackIdPriority.Add(cable.DestinationId);
                        }
                    }
                }
                return rackIdPriority;
            };

            var rackIdPriority = calculateRackPriority();

            Debug.WriteLine($"{string.Join(", ", rackIdPriority)}");

            var cablesSorted = cables.OrderBy(cable =>
                                    {
                                        int index = rackIdPriority.IndexOf(cable.DestinationId);
                                        return index < 0 ? rackIdPriority.Count : index; // index should never be < 0
                                    })
                                    .ThenBy(cable => cable.PanelId)
                                    .ToList();

            return cablesSorted;

        }

        internal static List<IdentifiedCableGroup> GroupByRoomOrLocation(List<IdentifiedCable> cables)
        {
            var groups = cables.GroupBy(idCable => {
                var sanitizedRoom = idCable.Cable.Room;
                var sanitizedLocation = idCable.Cable.Location;
                if (!string.IsNullOrWhiteSpace(sanitizedRoom))
                    return sanitizedRoom;
                else if (!string.IsNullOrWhiteSpace(sanitizedLocation))
                    return sanitizedLocation;
                else
                    return "No Room/Location";
                })
                .Select(group => new IdentifiedCableGroup(group.Key + "", [.. group]))
                .ToList();

            groups = groups.OrderBy(gr => gr.Name).ToList();
            return groups;
        }


        //Removed this method completely, after we looked at the data, we do not want to compare the racks by numeric only
        //They are sorted by quantity, and they need to be identified by teh full ID otherwise we will run into collisions
        //at some point and the result is not reliable.
        //private static int ExtractFirstNumber(string input)
        //{
        //    string numberStr = Regex.Match(input, @"\d+").Value;
        //    if (int.TryParse(numberStr, out var intValue))
        //    {
        //        return intValue;
        //    }

        //    return -1;
        //}
    }
}
