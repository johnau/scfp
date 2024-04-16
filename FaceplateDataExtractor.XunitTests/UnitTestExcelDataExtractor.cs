using FaceplateDataExtractor.Excel;
using System.Diagnostics;
using System.Reflection;

namespace FaceplateDataExtractor.XunitTests
{
    public class UnitTestExcelDataExtractor
    {
        [Fact]
        public void TestExtract()
        {
            var extractor = new MsExcelFaceplateDataExtractor();

            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resourceFolderPath = Path.Combine(assemblyDirectory!, "resources");
            var fileName = "20200320 - 1010 (0) CABLE SCHEDULE ken and marty6.xlsx";
            var filePath = Path.Combine(resourceFolderPath, fileName);
            Debug.WriteLine(filePath);
            var exists = File.Exists(filePath);
            Assert.True(exists);

            if (!extractor.TryExtractData(filePath, out var data))
            {
                Debug.WriteLine("Unable to extract data");
                return;
            }
        }
    }
}