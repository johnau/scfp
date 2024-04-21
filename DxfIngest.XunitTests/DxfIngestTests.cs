using System.Reflection;

namespace DxfIngest.XunitTests
{
    public class DxfIngestTests
    {
        [Fact]
        public void ReadDxf_WithBlocks_WillSuceced()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resourceFolderPath = Path.Combine(assemblyDirectory!, "resources");
            //var outputFolderPath = Path.Combine(assemblyDirectory!, "../../../output");
            var fileName = "dmx_female.dxf";
            var filePath = Path.Combine(resourceFolderPath, fileName);

            var ingestor = new BasicDxfIngestor();
            var dxfDrawing = ingestor.ImportDrawingFromFile(filePath);


        }
    }
}