using ExcelCableGeneratorApp.Dxf.Aggregates.Data;
using System.Diagnostics;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Helper;

/// <summary>
/// Temporary object to handle the CalculatePanelCombinations method
/// </summary>
internal class Combination
{
    private Dictionary<TechPanelSize, int> _panelSizes = [];
    private Dictionary<TechPanelSize, List<HashSet<string>>> _panelSizesToSystems = [];

    public int SocketQuantity { get; private set; }

    /// <summary>
    /// Reamining socket count to be homed on a panel
    /// </summary>
    public int Remainder { get; set; }

    /// <summary>
    /// Sizes of panels in this combo, with int Quantity as value.
    /// </summary>
    public Dictionary<TechPanelSize, int> PanelSizes => new Dictionary<TechPanelSize, int>(_panelSizes);

    /// <summary>
    /// Panel sizes with list of sets of system names that can fit onto that panel size
    /// - If a set contains only one system name, that system does not share the panel
    /// - If a set contains more than one system name, those systems share the panel
    /// </summary>
    public Dictionary<TechPanelSize, List<HashSet<string>>> PanelSizesToSystems => new Dictionary<TechPanelSize, List<HashSet<string>>>(_panelSizesToSystems);

    /// <summary>
    /// Whether or not this Combination is complete (all sockets homed)
    /// </summary>
    public bool IsDone => Remainder <= 0;

    /// <summary>
    /// Whether or not the panels in this combination can be arranged into a Rectangle
    /// </summary>
    public bool CanFormRectangle { get; private set; }

    /// <summary>
    /// The Remaining panel width (in U units) to make this combo rectangular.
    /// </summary>
    public int RemainderToMakeRectangular { get; private set; }

    /// <summary>
    /// Used to assess panel validity
    /// A higher value is worse
    /// </summary>
    public int VacantPanelSpace { get; private set; }

    /// <summary>
    /// Used to assess panel validty
    /// A negative rating represents a panel combo that is taller than it is wide (X axis in U units, and Y axis in Panel units... ie. 3 x 2GANG panels will result in a negative rating
    /// </summary>
    public int ShapeRating { get; private set; }
    /// <summary>
    /// Main constructor
    /// </summary>
    /// <param name="firstPanelSize"></param>
    /// <param name="useOneSize"></param>
    public Combination(TechPanelSize firstPanelSize, int totalSocketCount, bool useOneSize = false)
    {
        SocketQuantity = totalSocketCount;
        Remainder = totalSocketCount;
        IncrementPanelQuantity(firstPanelSize);
        VacantPanelSpace = 0;
        ShapeRating = 0;

        if (useOneSize)
            CalculateCombinationsOfOneSize(firstPanelSize);
    }
    
    private void CalculateCombinationsOfOneSize(TechPanelSize tps)
    {
        while (!IsDone)
        {
            Remainder -= tps.GetSocketQuantity();
            _panelSizes[tps] += 1;
        }
    }

    public void IncrementPanelQuantity(TechPanelSize tps)
    {
        if (IsDone)
            throw new Exception("Cannot increment the panel quantity as this combination is already done");

        if (_panelSizes.TryGetValue(tps, out var size))
        {
            Remainder -= tps.GetSocketQuantity();
            _panelSizes[tps] += 1;
            return;
        }

        _panelSizes[tps] = 1;
        Remainder -= tps.GetSocketQuantity();

        ProcessRectangleFit();
    }

    private void ProcessRectangleFit()
    {
        var widest = _panelSizes.Keys.Select(panelSize => panelSize.GetUWidth()).Max();

        var currentWidth = 0;

        // loop through all panel sizes
        foreach (var entry in _panelSizes)
        {
            var panelSize = entry.Key;
            var uWidth = panelSize.GetUWidth();
            var qty = entry.Value;

            for (int i = 1; i <= qty; i++)
            {
                // increment the currentWidth by the current panel size
                if (currentWidth + uWidth <= widest)
                    currentWidth += uWidth;

                // if we are at width, reset to 0
                if (currentWidth == widest)
                    currentWidth = 0;
            }
        }

        // if currentWidth is not 0 at the end, we dont have a rect
        RemainderToMakeRectangular = currentWidth == 0 ? 0 : widest - currentWidth;
        CanFormRectangle = RemainderToMakeRectangular == 0;
    }

    /// <summary>
    /// Method to evaluate whether this panel combination is suitable / give it a rating
    /// </summary>
    public void Evaluate()
    {
        // evaluate vacant space on panel array
        var totalSlots = _panelSizes.Select(p => p.Value * p.Key.GetSocketQuantity()).Sum();
        VacantPanelSpace = totalSlots - SocketQuantity;

        // calculate the shape rating
        // check if all panels are the same size
        if (_panelSizes.Count == 1)
        {
            var kvp = _panelSizes.First();
            var panelWidth = kvp.Key.GetUWidth();
            var height = kvp.Value;
            if (height > panelWidth)
            {
                ShapeRating = panelWidth - height;
            }
        }
        else // _panelSizes.Count will never be 0 as it is set in constructor
        {
            throw new NotImplementedException("At the moment we are not handling groups of different sized panels");
            //var diffSizePanelCount = _panelSizes.Count;
            
            //foreach (var ps in _panelSizes)
            //{
            //    var panelSize = ps.Key;
            //    var qty = ps.Value;
            //    for (int i = 0; i < qty; i++)
            //    {
                    
            //    }
            //}
        }
    }
}

internal class TechPanelFitCalculator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="systemGroupsOnPanel"></param>
    /// <returns></returns>
    public static List<Combination> CalculatePanelCombinations(List<SystemGroupContents> systemGroupsOnPanel)
    {
        //Dictionary<int, List<Combination>> panelsCombinations = [];
        List<Combination> panelsCombinations = [];

        foreach (var systemGroup in systemGroupsOnPanel)
        {
            //panelsCombinations.Add(systemGroup.SystemName, []);
            var socketsInGroup = systemGroup.Sockets.Count;
            var remainder = socketsInGroup;
            var sizes = Enum.GetValues(typeof(TechPanelSize));
            var sizesList = sizes.OfType<TechPanelSize>().ToList();

            // loop the panels from largest to smallest
            for (int i = sizesList.Count - 1; i > 0; i--)
            {
                var techPanelSize = sizesList[i];
                var size = (int)techPanelSize;

                // we also go through previous combinations before adding new ones
                for (int i2 = 0; i2 < panelsCombinations.Count; i2++)
                {
                    var other = panelsCombinations[i2];
                    if (other.IsDone)
                        continue;

                    other.Remainder -= size;
                    other.IncrementPanelQuantity(techPanelSize);
                }

                // each size of panel can start a new combination
                var oneSizeCombo = new Combination(techPanelSize, socketsInGroup, true);
                panelsCombinations.Add(oneSizeCombo);

                var combo = new Combination(techPanelSize, socketsInGroup);
                panelsCombinations.Add(combo);
                                
                remainder -= size;
                if (remainder <= 0)
                    break;
            }
        }

        return panelsCombinations;
    }

    /// <summary>
    /// Work out valid panel combinations for this group
    /// - A panel arrangement is valid if it can fit all the sockets without height exceeding width
    /// - One panel arrangment may be less valid than another (has more blank space), but this can be handled later
    /// - A panel arragement for both audio sockets alone and with other sockets should be considered for each TP
    /// --- These arragnements can be weighted later
    /// </summary>
    /// <param name="systemGroupsOnPanel"></param>
    /// <returns></returns>
    public static List<Combination> CalculatePanelCombinations2(List<SystemGroupContents> systemGroupsOnPanel)
    {
        List<Combination> panelsCombinations = [];

        var minPanelSize = DetermineMinimumPanelWidth(systemGroupsOnPanel);
        var totalSockets = systemGroupsOnPanel.Select(group => group.Sockets.Count()).Sum();

        for (TechPanelSize techPanelSize = TechPanelSize.TP_4GANG; techPanelSize >= minPanelSize; techPanelSize--)
        {
            //var techPanelSize = sizesList[tps];
            var size = techPanelSize.GetSocketQuantity();

            // each size of panel can start a new combination
            var oneSizeCombo = new Combination(techPanelSize, totalSockets, true);
            panelsCombinations.Add(oneSizeCombo);

            // evaluate panel based on free space (combination object can do this evaluiaotn)
            oneSizeCombo.Evaluate();
        }

        return panelsCombinations;
    }

    /// <summary>
    /// Find the smallest panel that will both fit an entire audio group on it,
    /// and will also not become taller than it is wide.
    /// </summary>
    /// <param name="systemGroupContents"></param>
    /// <returns></returns>
    public static TechPanelSize DetermineMinimumPanelWidth(List<SystemGroupContents> systemGroupContents)
    {
        var sizes = Enum.GetValues(typeof(TechPanelSize));
        var sizesList = sizes.OfType<TechPanelSize>().ToList();

        var totalSockets = systemGroupContents.Select(group => group.Sockets.Count).Sum();
        var totalAudioSockets = systemGroupContents
            .Where(group => group.SystemName.Contains("audio digital / analogue", StringComparison.CurrentCultureIgnoreCase))
            .Select(group => group.Sockets.Count).Sum();

        var minimumPanelWidthBasedOnAudioSockets = TechPanelSizeInfo.SmallestPanelFromCount(totalAudioSockets);

        //if (totalAudioSockets <= 4)
        //{

        //} else if (totalAudioSockets <= 8)
        //{

        //} else // more than 8 audio sockets - major limiting factor - determines the minimum width of the panels
        //{

        //}

        var lastFittingPanel = TechPanelSize.NO_SIZE;
        var lastRemainder = 0;

        for (int i = sizesList.Count - 1; i > 0; i--)
        {
            var techPanelSize = sizesList[i];
            var size = techPanelSize.GetSocketQuantity();

            if (minimumPanelWidthBasedOnAudioSockets > techPanelSize)
            {
                Debug.WriteLine($"The tech panel size : {techPanelSize} is too small to fit all the audio sockets on it");
                break;
            }

            var panelCount = (totalSockets / size);
            var remainder = totalSockets % size;

            if (panelCount > size && i < sizesList.Count - 1)
                break;

            lastFittingPanel = techPanelSize;
            lastRemainder = remainder;
        }

        return lastFittingPanel;
    }

    public static List<SystemGroupContents> SelectGroupsForPanel(List<SystemGroupContents> systemGroups, int panelSlotsCount, int panelRowCount, out List<SystemGroupContents> leftOverGroups)
    {
        if (panelSlotsCount % panelRowCount != 0)
            throw new Exception("There is an issue with the provided panel info (slot count and row count), it is not a grid.");

        var rowSize = panelSlotsCount / panelRowCount;
        //var grid = new bool[rowSize, panelRowCount];
        var grid = new List<List<string>>();
        for (int i = 0; i < panelRowCount; i++)
        {
            grid.Add([]);
        }

        var socketGroupsHoused = new List<SystemGroupContents>();
        //var socketGroupsRemaining = new List<SystemGroupContents>();

        // sort system groups into placement order
        var currentRow = 0;
        var freeRowSlots = rowSize;
        // iterate each system group, look at the socket count in that group
        // iterate with for loop, and every time we fit a group (ie. change the layout)
        // we want to go back the start and check the other groups if they fit now
        // .... or.. really we can do this when we switch rows.
        // ie. switch to next row, reset the loop...
        //foreach (var group in systemGroups)
        //
        // This loop is pretty ugly - so it fits in wit the rest of the codebase so far XD
        for (int i = 0; i < systemGroups.Count; i++)
        {
            var group = systemGroups[i];
            var sockets = group.Sockets;
            //var fits = false;
            // see if it will fit on a row
            if (freeRowSlots >= sockets.Count && !socketGroupsHoused.Contains(group))
            {
                // fits
                freeRowSlots -= sockets.Count;
                foreach (var socket in sockets) 
                {
                    grid[currentRow].Add(socket.Key);
                }
                //fits = true;
                socketGroupsHoused.Add(group);
            }

            if (freeRowSlots == 0) // go to next row when the row is complete if not at last row
            {
                if (currentRow == panelRowCount)
                {
                    // bail as we are full
                    break;
                }
                else
                {
                    currentRow++;// go to next row
                    i = 0;// go back to start of groups
                }

                freeRowSlots = rowSize; // reset free row slots
            }

            // if we are at the end of the groups, but we havent reached the end of the panel
            if (i == systemGroups.Count - 1 && currentRow < panelRowCount)
            {
                currentRow++;// go to next row
                i = 0;// go back to start of groups
            }
        }

        // filter out the groups that got added, to get just the remainder groups
        leftOverGroups = systemGroups.Where(group => !socketGroupsHoused.Contains(group)).ToList();
        return socketGroupsHoused;
    }

}
