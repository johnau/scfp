using netDxf.Header;
using netDxf;
using netDxf.Entities;
using System.Reflection;
using System.Diagnostics;

namespace DxfIngest
{
    public class BasicDxfIngestor : IDxfIngestor
    {
        public BasicDxfIngestor()
        {
        }

        public DxfDrawing? ImportDrawingFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new Exception("No file path provided");

            // this check is optional but recommended before loading a DXF file
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(filePath);
            // netDxf is only compatible with AutoCad2000 and higher DXF versions
            if (dxfVersion < DxfVersion.AutoCad2000) throw new Exception("Bad version");
            // load file
            DxfDocument loaded = DxfDocument.Load(filePath);

            //foreach (var block in loaded.Blocks)
            //{
            //    block.Name
            //}


            if (loaded.Blocks.TryGetValue("DMX_FEMALE_CUTOUT", out var blockItem))
            {
                
            }
            //throw new NotImplementedException();


            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var outputFolderPath = Path.Combine(assemblyDirectory!, "../../../output");
            Directory.CreateDirectory(outputFolderPath);
            var fileName = "test_out.dxf";
            var outputFilePath = Path.Combine(outputFolderPath, fileName);
            Debug.WriteLine($"Output path: {outputFilePath}");


            // create a new document, by default it will create an AutoCad2000 DXF version
            DxfDocument doc = new DxfDocument();
            // an entity
            Line entity = new Line(new Vector2(5, 5), new Vector2(10, 5));
            // add your entities here
            doc.Entities.Add(entity);
            // save to file
            //doc.Save(outputFilePath);

            return null;
        }

        public DxfDrawing? ImportDrawingFromFile(string filePath, ImportSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
