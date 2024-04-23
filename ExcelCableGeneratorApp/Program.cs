using ExcelCableGeneratorApp;
using ExcelCableGeneratorApp.Extract;
using ExcelCableGeneratorApp.Identifier.Aggregates;
using ExcelCableGeneratorApp.Output.Excel;
using ExcelCableGeneratorApp.Persistence;
using ExcelCableGeneratorApp.Persistence.Mapper;
using ExcelCableGeneratorApp.Sorting;
using ExcelCableGeneratorApp.Utility;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Stagecraft Cables Extractor (Excel)");

        var filePath = OnStart(args);
        var sheet = 1;

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

        var filteredCables = process.FilterData();


        //foreach (var scd in filteredCables)
        //    Console.WriteLine(scd);
        var sortedData = process.SortData();
        var identifiedData = process.AssignIdsToCables();
        //foreach (var idd in identifiedData)
        //{
        //    Console.WriteLine($"\n\nSystem Group: {idd.Name}\n\n");
        //    foreach (var cable in idd.Cables)
        //        Console.WriteLine(cable);
        //}

        var byRackData = process.GroupByRackAcrossEntireSystem();



        var filesCreated = WriteRackGroupsToFiles(byRackData);
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

    private static string WriteDataToExcel(List<IdentifiedCableGroup> cableGroups, string filename)
    {
        var writer = new ExcelWriter("./", filename);
        writer.CreateWorkbook();

        foreach (var group in cableGroups)
        {
            writer.WriteCablesToSpreadsheet(group.Name, group.Cables);
        }
        
        writer.FinalizeWorkbook();
        return writer.FilePath;
    }

    private static List<string> WriteRackGroupsToFiles(List<IdentifiedCableGroup> rackGroups)
    {
        var dir = "RackFiles";
        Directory.CreateDirectory(dir);
        List<string> filePaths = [];

        Dictionary<string, Dictionary<string, int>> rackCableQuants = [];

        foreach (var rack in rackGroups)
        {
            //var strippedGroupName = StringHelper.StripAllNonAlphanumericChars(rackGroup.Name);
            var fixedRackName = string.IsNullOrWhiteSpace(rack.Name) ? "NoDest" : rack.Name;
            var fileName = $"{dir}/{fixedRackName}.txt";
            
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

        string indexFileName = $"{dir}/Destinations.txt";
        List<string> fileLines = [];
        foreach (var rcq in rackCableQuants)
        {
            var rackName = rcq.Key;
            var systemsAndCableQuants = rcq.Value;
            var totalInRack = systemsAndCableQuants.Select(x => x.Value).Sum();
            fileLines.Add($"Destination: {rackName} ({totalInRack} cables total)");
            fileLines.Add("========================");

            foreach (var kvp in systemsAndCableQuants)
            {
                var keyTruncate = kvp.Key.Length > 30 ? kvp.Key[..30] : kvp.Key;
                fileLines.Add($"{keyTruncate,30}:\t\t {kvp.Value} Cables");
            }

            fileLines.Add("\n\n");
        }
        File.WriteAllLines(indexFileName, fileLines);
        filePaths.Add(indexFileName);

        return filePaths;
    }

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