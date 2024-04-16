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
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resourceFolderPath = Path.Combine(assemblyDirectory!, "resources");
            var fileName = "20200320 - 1010 (0) CABLE SCHEDULE ken and marty6.xlsx";
            var filePath = Path.Combine(resourceFolderPath, fileName);

            var extractor = new MsExcelFaceplateDataExtractor(filePath, 1);

            Debug.WriteLine(filePath);
            var exists = File.Exists(filePath);
            Assert.True(exists);

            if (!extractor.TryExtractData(0, out var data, out var rejectedData))
            {
                Debug.WriteLine("Unable to extract data");
                return;
            }
        }
    }
}