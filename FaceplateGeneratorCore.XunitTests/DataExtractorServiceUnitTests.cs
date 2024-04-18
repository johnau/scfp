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
            var fileName = "20200320 - 1010 (0) CABLE SCHEDULE ken and marty6.xlsx";
            var filePath = Path.Combine(resourceFolderPath, fileName);

            var service = new DataExtractorService();
            var cables = service.ExtractFromMasterExcelTemplate(filePath);
            var sortedList = cables.OrderBy(data => data.Id).ToList();

            var strings = new List<string>();

            foreach (var cable in sortedList)
            {
                Debug.WriteLine(cable);
                strings.Add(cable.ToString());
            }

            var filePathForDebug = Path.Combine(resourceFolderPath, "debug_output.txt");
            WriteStringsToFile(strings, filePathForDebug);
        }

        private static void WriteStringsToFile(List<string> strings, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                var header = $"{"",-10} {"Id",-10} {"Location",-28} {"Room",-13} {"Affl",-13} {"SourcePanelId",-13} {"DestinationPanelId",-13} {"CableType",-13}";
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
