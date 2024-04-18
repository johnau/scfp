using FaceplateDataExtractor.Excel;
using FaceplateDataExtractor.Utility;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace FaceplateDataExtractor.Model.Mapper
{
    /// <summary>
    /// Used to map the internal package entities to the final public data model
    /// </summary>
    /// <remarks>
    /// This class should possibly exist outside of the Model namespace, it refers to the .Excel
    /// namespace - Classes in the Model namespace should not have references outside of the Model
    /// namespace.
    /// </remarks>
    internal class ExtractedFaceplateDataMapper
    {
        /// <summary>
        /// Map a list of <see cref="WorksheetRowData"/> objects to a list of <see cref="ExtractedFaceplateData"/> objects
        /// </summary>
        /// <param name="rowDatas"></param>
        /// <returns></returns>
        public List<ExtractedFaceplateData> Map(List<WorksheetRowData> rowDatas)
        {
            var mapped = new List<ExtractedFaceplateData>();
            foreach (var rowData in rowDatas)
            {
                mapped.Add(Map(rowData));
            }

            return mapped;
        }

        /// <summary>
        /// Map a single row from <see cref="WorksheetRowData"/> to <see cref="ExtractedFaceplateData"/>
        /// </summary>
        /// <param name="rowData"></param>
        /// <returns></returns>
        public ExtractedFaceplateData Map(WorksheetRowData rowData)
        {
            var rowDataCollection = rowData.RowData;
            //var panelId = "";
            //var description = "";
            //var location = "";
            //var room = "";
            //var aboveFinishedFloorLevel = "";
            var model = new ExtractedFaceplateData();
            List<CableSystemData> cableSystemDatas = [];

            //ColumnGroupLayout? currentMatchedLayout = null;

            //foreach (var cell in rowDataCollection)
            var rowDataList = rowDataCollection.ToList();
            for (int i = 0; i < rowDataList.Count; i++)
            {
                var cellData = rowDataList[i].Value;

                // First we try to detect a non-system type column
                // If we match, we do not process the rest of the loop (continue)
                if (TryHandleMetadataColumn(cellData, out var metadataColumnType, out var descriptorColumnValue))
                {
                    switch (metadataColumnType)
                    {
                        case PanelDescriptorDataType.NONE:
                            throw new Exception("Internal Error: A NONE type column has been detected...");
                        case PanelDescriptorDataType.PANEL_ID:
                            model.PanelId = descriptorColumnValue;
                            break;
                        case PanelDescriptorDataType.DESCRIPTION:
                            model.Description = descriptorColumnValue;
                            break;
                        case PanelDescriptorDataType.LOCATION:
                            model.Location = descriptorColumnValue;
                            break;
                        case PanelDescriptorDataType.ROOM:
                            model.Room = descriptorColumnValue;
                            break;
                        case PanelDescriptorDataType.AFFL:
                            model.AboveFinishedFloorLevel = descriptorColumnValue;
                            break;
                        default:
                            throw new Exception("Internal Error: Unrecognized Metadata Column Type");
                    }
                    continue;
                }

                Debug.WriteLine($"{model}");

                // At this point we must be in the System Types column groups else the table/template is borked.
                // The LookAhead method will throw an exception if it can't find a match - can't continue further
                // and the user will need to address the issues
                // Need to make sure errors at this point are comprehensive as the user will most likely need to
                // edit or rebuild the spreadsheet.
                //if (currentMatchedLayout == null)
                //{
                    var currentMatchedLayout = LookAheadAndDetermineColumnLayout(i, rowDataList);
                    ReadColumnGroupValues(currentMatchedLayout, rowDataList.GetRange(i, currentMatchedLayout.ColumnCount));

                    if (!TryDetectSystemTypeInHeader(cellData.HeaderText, out var systemType))
                    {
                        Debug.WriteLine($"[WARN]: Could not detect a System Type for header: {StringsHelper.ListToString(cellData.HeaderText)}");
                    }

                    if (!TryDetectCableTypeInHeader(cellData.HeaderText, out var cableType))
                    {
                        Debug.WriteLine($"[WARN]: Could not detect a Cable Type for header: {StringsHelper.ListToString(cellData.HeaderText)}");
                    }
                //}

                // process header text to get Sysetm Type and Cable Type
                //var (systemType, cableType) = ProcessTypesFromHeader(cellData.HeaderText);


                //if (!TryDetectSystemColumnValueType(cellData.HeaderText, out var columnValueType))
                //{
                //    Debug.WriteLine($"[WARN]: Could not detect a ColumnValueType for header: {StringsHelper.ListToString(cellData.HeaderText)}");
                //}

                //switch (columnValueType)
                //{
                //    case ColumnValueType.NONE:
                //        // throw error? ignore column?
                //        break;
                //    case ColumnValueType.DESTINATION:
                //        // handle destitaion
                //        //ProcessDestination(cellData.HeaderText, cellData.Value);
                //        break;
                //    case ColumnValueType.QUANTITY_GENDERLESS:
                //        // handle normal quantity column
                //        //ProcessQuantity(cellData.HeaderText);
                //        break;
                //    case ColumnValueType.QUANTITY_MALE:
                //        // handle male and female specially?
                //        break;
                //    case ColumnValueType.QUANTITY_FEMALE:
                //        // handle male and female specially?
                //        break;
                //    default:
                //        throw new ArgumentException("Unrecognized ColumnValueType");

                //}

                var quantity = 0;
                var destination = "";
                var cableSystemData = new CableSystemData(systemType, cableType, quantity, destination);
                cableSystemDatas.Add(cableSystemData);

                //if (currentMatchedLayout.EndIndex == i)
                //{
                //    currentMatchedLayout = null;
                //}
            }

            return model;

            //return new ExtractedFaceplateData(panelId, description, location, room, aboveFinishedFloorLevel, cableSystemDatas);
        }


        private void ReadColumnGroupValues(ColumnGroupLayout columnGroupLayout, List<KeyValuePair<string, WorksheetCellData>> relevantCellList)
        {
            // populate the columngrouplayout with the values? and then populate the CableSystemData object
            // and then populate the ExtractedFaceplateData object
        }

        private ColumnGroupLayout LookAheadAndDetermineColumnLayout(int i, List<KeyValuePair<string, WorksheetCellData>> rowDataList)
        {
            var predefinedColumnLayouts = ColumnGroupLayout.ColumnLayouts();
            var matchingLayouts = new List<ColumnGroupLayout>();
            var reachedEndOfColumnGroup = false;
            // We expect this loop to exit within 2 or 3 iterations (depending on the column layout found)
            for (int ii = i; ii < rowDataList.Count; ii++)
            {
                var cellData = rowDataList[i].Value ?? throw new Exception("Should not get null WorksheetCellData object");

                if (!TryDetectSystemColumnValueType(cellData.HeaderText, out var columnValueType))
                {
                    Debug.WriteLine($"[WARN]: Could not detect a ColumnValueType for header: {StringsHelper.ListToString(cellData.HeaderText)}");

                    break;
                }

                // Iterate all of the predefined layouts and find all possible candidates
                foreach (var predefinedColLayout in predefinedColumnLayouts)
                {
                    if (predefinedColLayout.Contains(columnValueType))
                    {
                        if (matchingLayouts.Contains(predefinedColLayout))
                        {
                            reachedEndOfColumnGroup = true;
                            Debug.WriteLine($"Reached the end of {predefinedColLayout.GetType()} at index: {ii}");
                            break; // we've gone too far and hit another batch of columns
                        }

                        predefinedColLayout.StartIndex = i;
                        predefinedColLayout.SetIndexForColumnValueType(columnValueType, ii);
                        matchingLayouts.Add(predefinedColLayout);
                    } else
                    {
                        // If this predefined layout doesnt match, but is in the match list, remove it
                        if (matchingLayouts.Contains(predefinedColLayout))
                            matchingLayouts.Remove(predefinedColLayout);
                    }
                }

                if (reachedEndOfColumnGroup) 
                    break;

                //switch (columnValueType)
                //{
                //    case ColumnValueType.NONE:
                //        // throw error? ignore column?
                //        break;
                //    case ColumnValueType.DESTINATION:
                //        // handle destitaion
                //        //ProcessDestination(cellData.HeaderText, cellData.Value);
                //        break;
                //    case ColumnValueType.QUANTITY_GENDERLESS:
                //        // handle normal quantity column
                //        //ProcessQuantity(cellData.HeaderText);
                //        break;
                //    case ColumnValueType.QUANTITY_MALE:
                //        // handle male and female specially?
                //        break;
                //    case ColumnValueType.QUANTITY_FEMALE:
                //        // handle male and female specially?
                //        break;
                //    default:
                //        throw new ArgumentException("Unrecognized ColumnValueType");

                //}
            }
            // - at the first system types column we look for the structure
            // - we then check this structure agains the rest of the columns to see everyhting is the same
            // - whilst doing this, if we encounter a structure that is not the same, we try to handle with
            // one of the other known layouts.
            // - we can create some descriptors of these layouts for a default parse.
            // - 

            if (matchingLayouts.Count == 0 && matchingLayouts.Count > 1)
            {
                throw new Exception($"The column layout is not able to be determined, there are {matchingLayouts.Count} matches");
            }

            var matchedLayout = matchingLayouts.First();
            //matchedLayout.StartIndex = i;
            return matchedLayout;
        }

        private int ProcessQuantity(List<string> headerText)
        {
            var quantity = 0;

            return quantity;
        }

        private string ProcessDestination(List<string> headerText)
        {
            var destination = "UNKNOWN";

            return destination;
        }

        private static bool TryHandleMetadataColumn(WorksheetCellData? cellData, out PanelDescriptorDataType metadataType, out string value)
        {
            metadataType = default;
            value = "";

            if (cellData == null)
                return false;

            value = cellData.Value + "";

            var headerCombinedString = StringsHelper.ListToString(cellData.HeaderText);
            // check the column header against the ExpectedMetadataHeaders
            return ContainsMatchingType(headerCombinedString, out metadataType);
        }

        /// <summary>
        /// This method has the assumption that the header text is formatted correctly (ie. the order
        /// of the header strings is SystemType, CableType, and then either Quantity or Destination).
        /// * Note: This needs to be enforced somewhere earlier? Or the template needs to be strictly 
        /// controlled (make clear to the user which cells cannot be changed).  Ideally, a web interface
        /// will probably be preferable for the data entry to control user input from the start.
        /// </summary>
        /// <param name="headerParts"></param>
        /// <param name="systemType"></param>
        /// <returns></returns>
        private static bool TryDetectSystemTypeInHeader(List<string> headerParts, out SystemType systemType)
        {
            //systemType = SystemType.NONE;

            var firstHeader = "";

            // Get the first populated header from the list
            for (int i = 0; i < headerParts.Count; i++)
            {
                var part = headerParts[i];
                if (part == "") continue;
                
                firstHeader = part;
                break;
            }

            // could escape early here...

            return ContainsMatchingType(firstHeader, out systemType);

            //firstHeader = StringsHelper.Sanitize(firstHeader);

            //// Use string values assosciated with Enum values to determine a matching type
            //foreach (SystemType _type in Enum.GetValues(typeof(SystemType)))
            //{
            //    var possibleMatches = _type.GetStringArrayValue();
            //    for (int i = 0; i < possibleMatches.Length; i++)
            //    {
            //        if (firstHeader.Contains(possibleMatches[i], StringComparison.OrdinalIgnoreCase))
            //        {
            //            return true;
            //        }
            //    }
            //}

            //return false;

            // Use string values assosciated with Enum values to determine a matching type
            //foreach (SystemType _systemType in Enum.GetValues(typeof(SystemType)))
            //{
            //    var parts = _systemType.GetStringArrayValue();
            //    var matches = 0;
            //    for (int i = 0; i < parts.Length; i++)
            //    {
            //        if (!firstHeader.Contains(parts[i], StringComparison.CurrentCultureIgnoreCase))
            //            break;

            //        matches++;
            //    }

            //    if (matches != parts.Length)
            //        break;

            //    systemType = _systemType;
            //    return true;
            //}

            /* 
             * 
             * At this point we could also check other parts of the header...
             * But it would be nice to be able to assume that the header will always
             * be structured in the same way/order
             * 
             */

            //return false;
        }

        private static bool TryDetectCableTypeInHeader(List<string> headerParts, out CableType cableType)
        {
            //cableType = CableType.NONE;

            var secondHeader = "";

            // Get the second populated header from the list
            var hitFirst = false;
            for (int i = 0; i < headerParts.Count; i++)
            {
                var part = headerParts[i];
                if (part == "") continue;

                if (!hitFirst)
                {
                    hitFirst = true;
                    continue;
                }

                secondHeader = part;
                break;
            }

            return ContainsMatchingType(secondHeader, out cableType);

            //// Use string values assosciated with Enum values to determine a matching type
            //foreach (CableType _cableType in Enum.GetValues(typeof(CableType)))
            //{
            //    var parts = _cableType.GetStringArrayValue();
            //    var matches = 0;
            //    for (int i = 0; i < parts.Length; i++)
            //    {
            //        if (!secondHeader.Contains(parts[i], StringComparison.CurrentCultureIgnoreCase))
            //            break;

            //        matches++;
            //    }

            //    if (matches != parts.Length)
            //        break;

            //    cableType = _cableType;
            //    return true;
            //}

            ///* 
            // * 
            // * At this point we could also check other parts of the header...
            // * But it would be nice to be able to assume that the header will always
            // * be structured in the same way/order
            // * 
            // */

            //return false;
        }

        private static bool TryDetectSystemColumnValueType(List<string> headerParts, out ColumnValueType columnValueType)
        {
            //columnValueType = ColumnValueType.NONE;

            var lastHeader = "";
            // Get the last populated header from the list
            for (int i = headerParts.Count - 1; i >= 0; i--)
            {
                var part = headerParts[i];
                if (part == "") continue;

                lastHeader = part;
                break;
            }

            return ContainsMatchingType(lastHeader, out columnValueType);

            //// Use string values assosciated with Enum values to determine a matching type
            //foreach (ColumnValueType _columnValueType in Enum.GetValues(typeof(ColumnValueType)))
            //{
            //    var possibleMatches = _columnValueType.GetStringArrayValue();
            //    for (int i = 0; i < possibleMatches.Length; i++)
            //    {
            //        if (lastHeader.Contains(possibleMatches[i], StringComparison.OrdinalIgnoreCase))
            //        {
            //            return true;
            //        }
            //    }
            //}

            //return false;

            //foreach (ColumnValueType _columnValueType in Enum.GetValues(typeof(ColumnValueType)))
            //{
            //var parts = _columnValueType.GetStringArrayValue();
            //var matches = 0;
            //for (int i = 0; i < parts.Length; i++)
            //{
            //    if (!lastHeader.Contains(parts[i], StringComparison.CurrentCultureIgnoreCase))
            //        break;

            //    matches++;
            //}

            //if (matches != parts.Length)
            //    break;

            //columnValueType = _columnValueType;
            //return true;
            //}

            //return false;
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Enum type used with this method should ensure that a default value is considered.
        /// If no matches are found, the default value is returned.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns>Finds the longest match and returns, or returns Enum default value</returns>
        /// <exception cref="Exception"></exception>
        public static bool ContainsMatchingType<T>(string s, out T type) where T : Enum
        {
            s = StringsHelper.Sanitize(s);

            var matches = new Dictionary<T, string>();

            foreach (T _type in Enum.GetValues(typeof(T)))
            {
                var enumTypeValueOptions = _type.GetStringArrayValue();
                for (int i = 0; i < enumTypeValueOptions.Length; i++)
                {
                    var currentMatch = enumTypeValueOptions[i];
                    if (s.Contains(currentMatch, StringComparison.OrdinalIgnoreCase))
                    {
                        matches.Add(_type, currentMatch);
                    }
                }
            }

            if (matches.Count == 0)
            {
                type = default!; // ignoring nulls - enums should ensure NONE is first (= 0 = default)
                return false;
            }

            // sort the matches and find the one with the most matched characters.
            // This is a little janky since it's possible that two items match the same character
            // length and we can get the wrong one.
            // For now it should be ok, and most cases should be ok.
            // This can be reviewed when we shift the enums to the database.
            var longestMatch = "";
            T longestMatchValue = default!;
            foreach (var match in matches)
            {
                var e = match.Key;
                var str = match.Value;
                if (str.Length > longestMatch.Length)
                {
                    longestMatch = str;
                    longestMatchValue = e;
                } else if (str.Length == longestMatch.Length)
                {
                    throw new Exception($"The enum {typeof(T)} needs to be modified since there are two or more values that are colliding during detection. ({str} and {longestMatch})  This error was somewhat expected at some point...");
                }
            }

            type = longestMatchValue;
            return true;
        }
    }
}
