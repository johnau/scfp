using ExcelCableGeneratorApp.Extract.SimpleExtractor;
using System.Reflection;

namespace ExcelCableGeneratorApp.XunitTests
{
    public class ExcelCableCrawlerUnitTests
    {
        [Fact]
        public void TestHeaderCrawl_WillSucceed()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resourceFolderPath = Path.Combine(assemblyDirectory!, "resources");
            var fileName = "20200320 - 1010 (0) CABLE SCHEDULE ken and marty6.xlsx";
            var filePath = Path.Combine(resourceFolderPath, fileName);

            List<string> primaryHeaders = ["Panel Id", "Description", "Location", "Room", "AFFL"];
            var config = new ExcelCableCrawler.Configuration(primaryHeaders);
            var crawler = new ExcelCableCrawler(config);
            var sheet = 1;
            crawler.CrawlCableTable(filePath, sheet);
        }
    }
}