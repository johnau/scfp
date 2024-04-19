using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FaceplateDataExtractor.Model;
using FaceplateGeneratorCore.Data;
using FaceplateGeneratorCore.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FaceplateGeneratorCore.XunitTests
{
    public class DataExtractorServiceUnitTests
    {
        [Fact]
        public void ExtractData_WithDefaultTemplate_WillSucceed()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resourceFolderPath = Path.Combine(assemblyDirectory!, "resources");
            var outputFolderPath = Path.Combine(assemblyDirectory!, "../../../output");
            var fileName = "20200320 - 1010 (0) CABLE SCHEDULE ken and marty6.xlsx";
            var filePath = Path.Combine(resourceFolderPath, fileName);

            var service = new DataExtractorService();
            var cables = service.ExtractFromMasterExcelTemplate(filePath);
            var sortedById = cables.OrderBy(data => data.Id)
                                    .Select(cable => cable.ToString())
                                    .ToList();
            var sortedByRoom = cables.OrderBy(cable => cable.Room)
                                        .Select(cable => cable.ToString())
                                        .ToList();
            var sortedBySourcePanelId = cables.OrderBy(cable => cable.SourcePanelId)
                                                .ThenBy(cable => cable.Room)
                                                .Select(cable => cable.ToString())
                                                .ToList();
            var sortedByDestinationPanelId = cables.OrderBy(cable => cable.DestinationPanelId)
                                                    .Select(cable => cable.ToString())
                                                    .ToList();

            Directory.CreateDirectory(outputFolderPath);
            var filePathForDebug = Path.Combine(outputFolderPath, "cables_by_id.txt");
            WriteStringsToFile(sortedById, filePathForDebug);
            filePathForDebug = Path.Combine(outputFolderPath, "cables_by_room.txt");
            WriteStringsToFile(sortedByRoom, filePathForDebug);
            filePathForDebug = Path.Combine(outputFolderPath, "cables_by_source.txt");
            WriteStringsToFile(sortedBySourcePanelId, filePathForDebug);
            filePathForDebug = Path.Combine(outputFolderPath, "cables_by_dest.txt");
            WriteStringsToFile(sortedByDestinationPanelId, filePathForDebug);
        }

        private static void WriteStringsToFile(List<string> strings, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                var header = $"{"",-10} {"Id",-10} {"Description",-28} {"Location",-28} {"Room",-13} {"Affl",-13} {"SourcePanelId",-13} {"DestinationPanelId",-13} {"CableType",-13}";
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
