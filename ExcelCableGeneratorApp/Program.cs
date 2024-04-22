using ExcelCableGeneratorApp;
using ExcelCableGeneratorApp.Identifier.Aggregates;
using ExcelCableGeneratorApp.Output.Excel;
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
        } catch (Exception)
        {
            Console.WriteLine("The file is in use or inaccessible, the program will now exit, press any key to continue...");
            Console.ReadKey();
            return;
        }

        var filteredCables = process.FilterData();
        foreach (var scd in filteredCables)
            Console.WriteLine(scd);
        var sortedData = process.SortData();
        var identifiedData = process.AssignIdsToCables();
        foreach (var idd in identifiedData)
        {
            Console.WriteLine($"\n\nSystem Group: {idd.Name}\n\n");
            foreach (var cable in idd.Cables)
                Console.WriteLine(cable);
        }

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

        Console.Write("Please type the file path to the XSLX file: ");
        filePath = Console.ReadLine();
        if (!string.IsNullOrEmpty(filePath))
            filePath = filePath.Trim('"');

        if (File.Exists(filePath))
            return filePath;

        Console.WriteLine($"File does not exist: '{filePath}'");
        return OnStart([]);
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

}