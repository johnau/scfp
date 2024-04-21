using FaceplateGeneratorCore.Model.Cable;
using FaceplateGeneratorCore.Service;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FaceplateGeneratorCore.XunitTests
{
    public class DataExtractorServiceTests
    {
        [Fact]
        public void ExtractData_WithDefaultTemplate_WillSucceed()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resourceFolderPath = Path.Combine(assemblyDirectory!, "resources");
            var outputFolderPath = Path.Combine(assemblyDirectory!, "..\\..\\..\\output");
            var fileName = "20200320 - 1010 (0) CABLE SCHEDULE ken and marty6.xlsx";
            var filePath = Path.Combine(resourceFolderPath, fileName);

            var service = new DataExtractorService();
            var systemGroups = service.ExtractFromMasterExcelTemplate(filePath);

            foreach (var group in systemGroups)
            {
                OutputCables(group.CablesInSystem, outputFolderPath, MakeValidFileName(group.SystemType.ToString()));
            }
        }

        public static string MakeValidFileName(string name)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars());
            string regexPattern = "[" + Regex.Escape(invalidChars) + "]";
            return Regex.Replace(name, regexPattern, "_");
        }

        private static void OutputCables(List<CableData> cables, string outputFolderPath, string fileName)
        {
            var filledCables = GenerateEmptyPlaceholderCables(cables);
            var cableStrings = filledCables
                .Select(cable => cable.ToString())
                .ToList();
            var cableCsvStrings = filledCables
                .Select(cable => cable.ToCsvString())
                .ToList();
            Directory.CreateDirectory(outputFolderPath);

            var filePathForDebug = Path.Combine(outputFolderPath, fileName + ".txt");
            WriteStringsToFile(cableStrings, filePathForDebug);
            var filePathForDebugCsv = Path.Combine(outputFolderPath, fileName + ".csv");
            WriteStringsToCsvFile(cableCsvStrings, filePathForDebugCsv);
        }

        public static List<CableData> GenerateEmptyPlaceholderCables(List<CableData> cables)
        {
            if (cables.Count == 0) return [];

            var firstCable = cables.First();
            var prefix = firstCable.Id[..(FindFirstDigitIndex(firstCable.Id))];

            List<CableData> cableDataFillEmpties = [];
            var _cables = cables.OrderBy(cable => cable.Id).ToList(); // should already be soretd this way / ensure
            List<int> numericIds = _cables.Select(cable => cable.IdNumberOnly).ToList();

            int lastIdNumber = _cables[0].IdNumberOnly;
            cableDataFillEmpties.Add(_cables[0]);

            for (int i = 1; i < _cables.Count; i++)
            {
                var cable = _cables[i];
                if (cable.IdNumberOnly - lastIdNumber == 1)
                {
                    cableDataFillEmpties.Add(cable);
                    lastIdNumber = cable.IdNumberOnly;
                } 
                else if (cable.IdNumberOnly - lastIdNumber > 1)
                {
                    for (int ii = lastIdNumber + 1; ii < cable.IdNumberOnly; ii++)
                    {
                        string id = $"{prefix}{ii}";
                        cableDataFillEmpties.Add(CableData.PLACEHOLDER(id));
                        Debug.WriteLine($"Created Placeholder ID: `{prefix}` => {ii}");
                        lastIdNumber = ii;
                    }
                    cableDataFillEmpties.Add(cable);
                    lastIdNumber = cable.IdNumberOnly;
                }
            }

            return cableDataFillEmpties;
        }

        private static int FindFirstDigitIndex(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    return i;
                }
            }
            // If no digit is found, return -1 or throw an exception, depending on your requirements.
            return -1;
        }

        private static void WriteStringsToFile(List<string> strings, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                var header = $"{"",-10} {"Id",-10} {"Description",-28} {"Location",-28} {"Room",-13} {"Affl",-13} {"Panel Id",-16} {"Dest Rack Id",-14} {"SystemType",-13}";

                writer.WriteLine(header);

                foreach (var s in strings)
                {
                    writer.WriteLine(s);
                }
            }
            Debug.WriteLine($"File written to: {filePath}");
        }

        private static void WriteStringsToCsvFile(List<string> strings, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                var header = $"CableId, Panel Id, Description, Location, Room, Affl, Dest Rack Id, CableId, Qty, Keystone";

                writer.WriteLine(header);

                foreach (var s in strings)
                {
                    writer.WriteLine(s);
                }
            }
            Debug.WriteLine($"File written to: {filePath}");
        }
    }
}
