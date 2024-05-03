using DocumentFormat.OpenXml.Bibliography;
using ExcelCableGeneratorApp.Dxf.Aggregates.Data;
using ExcelCableGeneratorApp.Dxf.Drawing.Element;
using ExcelCableGeneratorApp.Identifier.Aggregates;

namespace ExcelCableGeneratorApp.Convert;

internal class CableToSocketConverter
{
    public CableToSocketConverter()
    {
    }

    /// <summary>
    /// Converts IdentifiedCableGroup objects into data for socket
    /// </summary>
    /// <param name="cablesGroupedBySource"></param>
    /// <returns>A Dictionary with Panel Id Key and Dictionary Value.  Value Dictionary contains entries of string Key Socket Id and SocketType value</returns>
    public static List<SourcePanelContents> ConvertCableGroupsToSockets(List<IdentifiedCableGroup> cablesGroupedBySource)
    {
        List<SourcePanelContents> converted = [];
        foreach (var cablesFromSource in cablesGroupedBySource)
        {
            var sourceName = cablesFromSource.Name;
            var spc = new SourcePanelContents
            {
                SourcePanelId = sourceName
            };
            converted.Add(spc);
            //if (!converted.TryAdd(sourceName, []))
            //{
            //    throw new Exception($"There is more than one source with id: {sourceName}");
            //}

            var cablesInGroup = cablesFromSource.Cables;
            var systemGroups = cablesInGroup.GroupBy(idc => idc.Cable.SystemType).ToList()
                                .Select(group => new
                                {
                                    Name = group.Key,
                                    Cables = group.ToList()
                                })
                                .ToList();

            foreach (var cableSystemGroup in systemGroups)
            {
                var systemType = cableSystemGroup.Name;
                var cablesInSystem = cableSystemGroup.Cables;

                var systemGroup = new SystemGroupContents()
                {
                    SystemName = systemType
                };

                foreach (var cable in cablesInSystem)
                {
                    var cableId = cable.Id.IdFull;
                    var cableType = cable.Cable.CableType;
                    var qtyType = cable.Cable.QuantityType;
                    var socketType = SocketTypeFromSystemTypeString(systemType, cableType, qtyType);

                    systemGroup.Sockets.TryAdd(cableId, socketType);
                    
                    //if (!converted[sourceName].TryAdd(cableId, socketType))
                    //{
                    //    throw new Exception($"There are more than one cable with id: {cableId}");
                    //}
                }
                spc.SystemGroups.Add(systemGroup);
            }
        }

        return converted;
    }

    public static SocketFormat SocketTypeForCable(IdentifiedCable cableWithId)
    {
        var cableType = cableWithId.Cable.CableType;
        var systemType = cableWithId.Cable.SystemType;
        var qtyType = cableWithId.Cable.QuantityType;
        var socketFormat = SocketTypeFromSystemTypeString(systemType, cableType, qtyType);

        return socketFormat;
    }

    // Just want to know if the socket is type A or type D here.
    public static SocketFormat SocketTypeFromSystemTypeString(string systemType, string cableType, string quantityType)
    {
        // TRY WITH CABLE TYPE
        if (cableType.Contains("Cat 6A", StringComparison.CurrentCultureIgnoreCase))                    return SocketFormat.TYPE_A;
        if (cableType.Contains("Belden", StringComparison.CurrentCultureIgnoreCase))
        {
            if (quantityType.Contains("Send", StringComparison.CurrentCultureIgnoreCase))               return SocketFormat.TYPE_A;
            else if (quantityType.Contains("Return", StringComparison.CurrentCultureIgnoreCase))        return SocketFormat.TYPE_A;
            else                                                                                        return SocketFormat.TYPE_A;  // same as return but leave as a separate statement for clarity
        }
        if (cableType.Contains("AWG", StringComparison.CurrentCultureIgnoreCase))                       return SocketFormat.TYPE_A;
        if (cableType.Contains("0M3", StringComparison.CurrentCultureIgnoreCase))                       return SocketFormat.TYPE_A;
        if (cableType.Contains("OM3", StringComparison.CurrentCultureIgnoreCase))                       return SocketFormat.TYPE_A;
        if (cableType.Contains("3G-SDI-HD", StringComparison.CurrentCultureIgnoreCase))                 return SocketFormat.TYPE_A;

        // TRY WITH SYSTEM TYPE
        if (systemType.Contains("techincal data", StringComparison.CurrentCultureIgnoreCase))           return SocketFormat.TYPE_A;
        if (systemType.Contains("multimode fiber", StringComparison.CurrentCultureIgnoreCase))          return SocketFormat.TYPE_A;
        if (systemType.Contains("video tie line", StringComparison.CurrentCultureIgnoreCase))           return SocketFormat.TYPE_A;
        if (systemType.Contains("digital media", StringComparison.CurrentCultureIgnoreCase))            return SocketFormat.TYPE_A;
        if (systemType.Contains("av control", StringComparison.CurrentCultureIgnoreCase))               return SocketFormat.TYPE_A;
        if (systemType.Contains("audio digital / analogue", StringComparison.CurrentCultureIgnoreCase))
        {
            if (quantityType.Contains("Send", StringComparison.CurrentCultureIgnoreCase))               return SocketFormat.TYPE_A;
            else if (quantityType.Contains("Return", StringComparison.CurrentCultureIgnoreCase))        return SocketFormat.TYPE_A;
            else                                                                                        return SocketFormat.TYPE_A;
        }
        if (systemType.Contains("ethernet audio", StringComparison.CurrentCultureIgnoreCase))           return SocketFormat.TYPE_A;
        if (systemType.Contains("dante", StringComparison.CurrentCultureIgnoreCase))                    return SocketFormat.TYPE_A;
        if (systemType.Contains("talkback", StringComparison.CurrentCultureIgnoreCase))                 return SocketFormat.TYPE_A;
        if (systemType.Contains("performance relay input", StringComparison.CurrentCultureIgnoreCase))  return SocketFormat.TYPE_A;
        if (systemType.Contains("paging station", StringComparison.CurrentCultureIgnoreCase))           return SocketFormat.TYPE_A;
        if (systemType.Contains("performance loudspeaker", StringComparison.CurrentCultureIgnoreCase))  return SocketFormat.TYPE_A;
        if (systemType.Contains("stage lighting control", StringComparison.CurrentCultureIgnoreCase))   return SocketFormat.TYPE_A;
        if (systemType.Contains("work light control", StringComparison.CurrentCultureIgnoreCase))       return SocketFormat.TYPE_A;
        if (systemType.Contains("hoist control", StringComparison.CurrentCultureIgnoreCase))            return SocketFormat.TYPE_A;
        if (systemType.Contains("house curtain control", StringComparison.CurrentCultureIgnoreCase))    return SocketFormat.TYPE_A;
        if (systemType.Contains("pendent control", StringComparison.CurrentCultureIgnoreCase))          return SocketFormat.TYPE_A;
        if (systemType.Contains("estop", StringComparison.CurrentCultureIgnoreCase))                    return SocketFormat.TYPE_A;
        if (systemType.Contains("stage lighting outlets single", StringComparison.CurrentCultureIgnoreCase)) return SocketFormat.POW_GPO_SINGLE;
        if (systemType.Contains("work light outlets", StringComparison.CurrentCultureIgnoreCase))       return SocketFormat.POW_GPO_DOUBLE;
        if (systemType.Contains("10a 'dirty' double outlet", StringComparison.CurrentCultureIgnoreCase)) return SocketFormat.POW_GPO_SINGLE;
        if (systemType.Contains("10a audio power double outlet", StringComparison.CurrentCultureIgnoreCase)) return SocketFormat.POW_GPO_DOUBLE;
        if (systemType.Contains("3 phase outlet", StringComparison.CurrentCultureIgnoreCase))           return SocketFormat.POW_3_PHASE_SINGLE;

        return SocketFormat.NONE;
    }
}
