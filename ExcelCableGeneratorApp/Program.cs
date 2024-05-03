using DocumentFormat.OpenXml.Bibliography;
using ExcelCableGeneratorApp;
using ExcelCableGeneratorApp.Convert;
using ExcelCableGeneratorApp.Dxf;
using ExcelCableGeneratorApp.Extract;
using ExcelCableGeneratorApp.Identifier.Aggregates;
using ExcelCableGeneratorApp.Output.Excel;
using ExcelCableGeneratorApp.Persistence;
using ExcelCableGeneratorApp.Persistence.Mapper;
using ExcelCableGeneratorApp.Sorting;
using ExcelCableGeneratorApp.Utility;
using netDxf;
using System.Diagnostics;

/// <summary>
/// This is a scratch of the application process flow
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Stagecraft Cables Extractor (Excel)");

        var filePath = OnStart(args);
        var sheet = 1;

        Console.WriteLine("Compiling cables...");

        var process = new DataProcessHandler(filePath, sheet);
        process.LoadSettings("settings.json");
        try
        {
            var cables = process.ProcessFile();
        } 
        catch (Exception)
        {
            Console.WriteLine("The file is in use or inaccessible, the program will now exit, press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Filtering cables...");
        var filteredCables = process.FilterData();


        //foreach (var scd in filteredCables)
        //    Console.WriteLine(scd);
        Console.WriteLine("Sorting cables...");
        var sortedData = process.SortData();

        Console.WriteLine("Identifying cables...");
        var identifiedData = process.AssignIdsToCables();
        
        // 

        Console.WriteLine("Grouping cables by source and destination...");
        var groupedByDestination = process.GroupByDestinationAcrossEntireSystem();
        var groupedBySource = process.GroupBySourceAcrossEntireSystem();
        var groupedByRoom = process.GroupByRoomOrLocationAcrossEntireSystem();

        var generatedPanels = process.GenerateTechPanels(groupedBySource);

        foreach (var gp in generatedPanels)
        {
            var panels = gp.Value;
            for (int i = 0; i < panels.Count; i++)
            {
                var p = panels[i];
                DxfDocument doc = new();
                p.Draw(doc.Entities);
                doc.Save($"./generated_tech_panels/{gp.Key}_{i}.dxf");
            }
        }

        Console.WriteLine("Writing groups to excel...");
        var destExcelFile = WriteDataToExcel(groupedByDestination, "RackFiles/Destinations", true); //WriteGroupsToSpreadsheets(groupedByDestination, "Destinations");
        Console.WriteLine($"Wrote to: {destExcelFile}");
        //WriteGroupsToSpreadsheets(groupedBySource, "Sources");
        //var sourceExcelFile = WriteDataToExcel(groupedBySource, "RackFiles/Sources", true);
        var sourceFiles = WriteSourceGroupingsToFiles(groupedBySource, "Sources");
        Console.WriteLine($"Wrote to: {string.Join(", ", sourceFiles)}");

        var roomFiles = WriteRoomGroupingsToFiles(groupedByRoom, "Loc");
        Console.WriteLine($"Wrote to: {string.Join(", ", roomFiles)}");
        // DRAW DXF

        /**
         * 
         * Files and Database Stuff Below
         * 
         */
        Console.WriteLine("Writing groups to text files");
        var filesCreated = WriteGroupsToFiles(groupedByDestination, "Destination");
        Console.WriteLine($"Created rack files: {string.Join(", ", filesCreated)}");

        // write data to spreadsheet and csv
        var outFilePath = WriteDataToExcel(identifiedData, "test_out_cables");

        Console.WriteLine($"Saved file to: '{outFilePath}'");

        if (File.Exists(outFilePath))
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = outFilePath;
            psi.UseShellExecute = true;
            Process.Start(psi);
        }

        var databaseManager = new DatabaseManager();

        var (doSave, jobName) = PromptUserSaveToDatabase();
        if (!doSave)
        {
            return; // exit app
        }

        // save to database
        SaveToDatabase(jobName, databaseManager, identifiedData);

        var allJobNames = databaseManager.AllJobNames();
        Console.WriteLine($"Jobs in db: {string.Join(", ", allJobNames)}");

        if (allJobNames.Count > 0)
        {
            var testJobOutput = databaseManager.Read(allJobNames[0]);
            if (testJobOutput == null)
                Console.WriteLine($"Couldnt find any jobs");
            else
                Console.WriteLine($"Number of cables in job: {testJobOutput.Cables.Count}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void WriteDataToFiles()
    {

    }

    private static (bool, string) PromptUserSaveToDatabase()
    {
        Console.WriteLine($"Would you like to save this job? (Y/N)");
        var key = Console.ReadKey();
        switch (key.Key)
        {
            case ConsoleKey.Y:
                var jobName = "";
                while (string.IsNullOrWhiteSpace(jobName))
                {
                    Console.WriteLine("Please enter a unique job name:");
                    jobName = Console.ReadLine();
                }

                return (true, jobName);
            case ConsoleKey.N:
                Console.WriteLine("Exiting...");
                return (false, "");
            default:
                Console.WriteLine("Invalid input... Press Y for Yes or N for No...");
                return PromptUserSaveToDatabase();
        }
    }

    private static bool SaveToDatabase(string jobName, DatabaseManager database, List<IdentifiedCableGroup> cableGroup)
    {
        var newJobEntity = JobToEntityMapper.MapToEntity(jobName, cableGroup);
        var success = database.Create(newJobEntity);

        return success;
    }

    /// <summary>
    /// If file was dropped on .exe or as arg in terminal, args[0] is used
    /// Otherwise user is prompt to enter the file path to the xlsx file
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private static string OnStart(string[] args)
    {
        var filePath = "";
        if (args.Length > 0)
        {
            filePath = args[0];
            filePath = filePath.Trim('"');
            if (File.Exists(filePath))
                return filePath;
            else
                Console.WriteLine($"File does not exist: '{filePath}'");
        }

        Console.WriteLine("Press 'S' to go to Settings" +
            "\nPress any other key to run program...");
        switch (Console.ReadKey().Key)
        {
            case ConsoleKey.S:
                DisplaySettings();
                break;
            default:
                break;
        }

        Console.Write("Please type the file path to the XSLX file: ");
        filePath = Console.ReadLine();
        if (!string.IsNullOrEmpty(filePath))
            filePath = filePath.Trim('"');

        if (File.Exists(filePath))
            return filePath;

        Console.WriteLine($"File does not exist: '{filePath}'");
        return OnStart([]);
    }

    private static void DisplaySettings()
    {
        Console.WriteLine("Settigns...");

    }

    private static string WriteDataToExcel(List<IdentifiedCableGroup> cableGroups, string filename, bool excludeSpare = false)
    {
        var writer = new ExcelWriter("./", filename);
        writer.CreateWorkbook();

        foreach (var group in cableGroups)
        {
            writer.WriteCablesToSpreadsheet(group.Name, group.Cables, excludeSpare);
        }
        
        writer.FinalizeWorkbook();
        return writer.FilePath;
    }

    private static List<string> WriteGroupsToFiles(List<IdentifiedCableGroup> groups, string groupType)
    {
        var dir = "RackFiles";
        Directory.CreateDirectory(dir);
        List<string> filePaths = [];

        Dictionary<string, Dictionary<string, int>> rackCableQuants = [];

        foreach (var rack in groups)
        {
            //var strippedGroupName = StringHelper.StripAllNonAlphanumericChars(rackGroup.Name);
            var fixedRackName = string.IsNullOrWhiteSpace(rack.Name) ? "No" + groupType : rack.Name;
            var fileName = $"{dir}/{fixedRackName}_{groupType}.txt";
            
            List<string> cableStrings = [];

            foreach (var identifiedCable in rack.Cables)
            {
                var id = identifiedCable.Id.IdFull;
                var cable = identifiedCable.Cable;
                var cableString = $"{id}: {cable}";
                cableStrings.Add(cableString);
            }

            File.WriteAllLines(fileName, cableStrings);
            filePaths.Add(fileName);

            rackCableQuants.TryAdd(rack.Name, MapCableTypesToQuantityOf(rack.Cables));
        }

        string indexFileName = $"{dir}/{groupType}s.txt";
        List<string> fileLines = [];
        foreach (var rcq in rackCableQuants)
        {
            var rackName = rcq.Key;
            var systemsAndCableQuants = rcq.Value;
            var totalCables = systemsAndCableQuants.Select(x => x.Value).Sum();
            fileLines.Add($"{groupType}: {rackName} ({totalCables} cables total)");
            fileLines.Add("========================");

            foreach (var kvp in systemsAndCableQuants)
            {
                var keyTruncate = kvp.Key.Length > 30 ? kvp.Key[..30] : kvp.Key;
                fileLines.Add($"{keyTruncate,30}:\t\t {kvp.Value} Cables");
            }

            fileLines.Add("\n");
        }
        File.WriteAllLines(indexFileName, fileLines);
        filePaths.Add(indexFileName);

        return filePaths;
    }

    private static List<string> WriteRoomGroupingsToFiles(List<IdentifiedCableGroup> groups, string groupType)
    {
        var dir = "ByRoomFiles";
        Directory.CreateDirectory(dir);
        List<string> filePaths = [];

        Dictionary<string, Dictionary<string, int>> rackCableQuants = [];
        
        // loop through groups (by room)
        foreach (var roomGroup in groups)
        {
            var cables = roomGroup.Cables;
            var cableTypeGroups = cables.GroupBy(c => c.Cable.CableType)
                .Select(group => new {
                    Name = group.Key,
                    Cables = group.ToList()
                })
                .OrderBy(grp => grp.Name) // order the groups by name (cable type)
                .ToList();

            //var strippedGroupName = StringHelper.StripAllNonAlphanumericChars(rackGroup.Name);
            var fixedDest = string.IsNullOrWhiteSpace(roomGroup.Name) ? "No" + groupType : roomGroup.Name;
            fixedDest = StringHelper.StripAllNonAlphanumericChars(fixedDest);
            var fileName = $"{dir}/{fixedDest}_{groupType}.txt";

            List<string> cableStrings = [];
            cableStrings.Add(roomGroup.Name);
            cableStrings.Add("===================================");

            foreach (var cableTypeGroup in cableTypeGroups)
            {
                var count = 1;
                var _cabName = string.IsNullOrWhiteSpace(cableTypeGroup.Name) ? "No Spec" : cableTypeGroup.Name;

                cableStrings.Add($"\nCABLES:: {cableTypeGroup.Cables.Count}x {_cabName}");
                cableStrings.Add(new string('-', 60));
                foreach (var identifiedCable in cableTypeGroup.Cables)
                {
                    var id = identifiedCable.Id.IdFull;
                    var cable = identifiedCable.Cable;

                    var cableString = "";
                    if (cable.QuantityType.Contains("SEND", StringComparison.OrdinalIgnoreCase))
                    {
                        cableString = $"{count}) {id,-7} (M) | On: {cable.PanelId,-10} | \"{cable.Description}\" @ {cable.Location}, {cable.Room}";
                    }
                    else if (cable.QuantityType.Contains("RETURN", StringComparison.OrdinalIgnoreCase))
                    {
                        cableString = $"{count}) {id,-7} (F) | On: {cable.PanelId,-10} | \"{cable.Description}\" @ {cable.Location}, {cable.Room}";
                    }
                    else
                    {
                        cableString = $"{count}) {id,-7} | On: {cable.PanelId,-10} | \"{cable.Description}\" @ {cable.Location}, {cable.Room}";
                    }

                    cableStrings.Add(cableString);

                    count++;
                }
                cableStrings.Add(new string('=', 60));
            }

            File.WriteAllLines(fileName, cableStrings);
            filePaths.Add(fileName);

            rackCableQuants.TryAdd(roomGroup.Name, MapCableTypesToQuantityOf(roomGroup.Cables));
        }

        string indexFileName = $"{dir}/{groupType}s.txt";
        List<string> fileLines = [];
        foreach (var rcq in rackCableQuants)
        {
            var rackName = rcq.Key;
            var systemsAndCableQuants = rcq.Value;
            var totalCables = systemsAndCableQuants.Select(x => x.Value).Sum();
            fileLines.Add($"{groupType}: {rackName} ({totalCables} cables total)");
            fileLines.Add("========================");

            foreach (var kvp in systemsAndCableQuants)
            {
                var keyTruncate = kvp.Key.Length > 30 ? kvp.Key[..30] : kvp.Key;
                fileLines.Add($"{keyTruncate,30}:\t\t {kvp.Value} Cables");
            }

            fileLines.Add("\n");
        }
        File.WriteAllLines(indexFileName, fileLines);
        filePaths.Add(indexFileName);

        return filePaths;
    }

    /// <summary>
    /// Takes a list of ICG (grouped by source panel id)
    /// Produces a series of text files (1 per source panel id)
    /// Each text file contains the 
    /// - Name of the panel
    /// - Cables coming to/from this panel (grouped by calbe type)
    /// - Quantities of each cable type next to the group
    /// </summary>
    /// <param name="groups"></param>
    /// <param name="groupType"></param>
    /// <returns></returns>
    private static List<string> WriteSourceGroupingsToFiles(List<IdentifiedCableGroup> groups, string groupType)
    {
        var dir = "SourcePanelFiles";
        Directory.CreateDirectory(dir);
        List<string> filePaths = [];

        Dictionary<string, Dictionary<string, int>> rackCableQuants = [];

        // loop the groups, group the group contents by cable type
        foreach (var group in groups)
        {
            var cables = group.Cables;
            var cableTypeGroups = cables.GroupBy(c => c.Cable.CableType)
                .Select(group => new {
                        Name = group.Key,
                        Cables = group.ToList()
                    })
                .OrderBy(grp => grp.Name) // order the groups by name (cable type)
                .ToList();

            //var strippedGroupName = StringHelper.StripAllNonAlphanumericChars(rackGroup.Name);
            var fixedDest = string.IsNullOrWhiteSpace(group.Name) ? "No" + groupType : group.Name;
            var fileName = $"{dir}/{fixedDest}_{groupType}.txt";

            List<string> cableStrings = [];
            cableStrings.Add(group.Name);
            cableStrings.Add("===================================");

            foreach (var cableTypeGroup in cableTypeGroups)
            {
                var count = 1;
                var _cabName = string.IsNullOrWhiteSpace(cableTypeGroup.Name) ? "No Spec" : cableTypeGroup.Name;
                
                cableStrings.Add($"\nCABLES:: {cableTypeGroup.Cables.Count}x {_cabName}");
                cableStrings.Add(new string('-', 60));
                foreach (var identifiedCable in cableTypeGroup.Cables)
                {
                    var id = identifiedCable.Id.IdFull;
                    var cable = identifiedCable.Cable;

                    var cableString = "";
                    if (cable.QuantityType.Contains("SEND", StringComparison.OrdinalIgnoreCase))
                    {
                        cableString = $"{count}) {id,-7} (M) | On: {cable.PanelId,-10} | \"{cable.Description}\" @ {cable.Location}, {cable.Room}";
                    } else if (cable.QuantityType.Contains("RETURN", StringComparison.OrdinalIgnoreCase))
                    {
                        cableString = $"{count}) {id,-7} (F) | On: {cable.PanelId,-10} | \"{cable.Description}\" @ {cable.Location}, {cable.Room}";
                    } else
                    {
                        cableString = $"{count}) {id,-7} | On: {cable.PanelId,-10} | \"{cable.Description}\" @ {cable.Location}, {cable.Room}";
                    }
                    
                    cableStrings.Add(cableString);

                    count++;
                }
                cableStrings.Add(new string('=', 60));
            }

            File.WriteAllLines(fileName, cableStrings);
            filePaths.Add(fileName);

            rackCableQuants.TryAdd(group.Name, MapCableTypesToQuantityOf(group.Cables));
        }

        string indexFileName = $"{dir}/{groupType}s.txt";
        List<string> fileLines = [];
        foreach (var rcq in rackCableQuants)
        {
            var rackName = rcq.Key;
            var systemsAndCableQuants = rcq.Value;
            var totalCables = systemsAndCableQuants.Select(x => x.Value).Sum();
            fileLines.Add($"{groupType}: {rackName} ({totalCables} cables total)");
            fileLines.Add("========================");

            foreach (var kvp in systemsAndCableQuants)
            {
                var keyTruncate = kvp.Key.Length > 30 ? kvp.Key[..30] : kvp.Key;
                fileLines.Add($"{keyTruncate,30}:\t\t {kvp.Value} Cables");
            }

            fileLines.Add("\n");
        }
        File.WriteAllLines(indexFileName, fileLines);
        filePaths.Add(indexFileName);

        return filePaths;
    }

    //private static string WriteGroupsToSpreadsheets(List<IdentifiedCableGroup> groups, string groupType)
    //{
    //    return WriteDataToExcel(groups, groupType);
    //}

    private static Dictionary<string, int> MapCableTypesToQuantityOf(List<IdentifiedCable> cablesInRack)
    {
        Dictionary<string, int> mapped = [];

        var sysGroups = SortHelper.GroupIdentifiedBySystem(cablesInRack);
        foreach (var sys in sysGroups)
        {
            mapped.Add(sys.Name, sys.Cables.Count);
        }
        return mapped;
    }
}