using netDxf.Header;
using netDxf;
using netDxf.Entities;

namespace DxfIngest
{
    public class BasicDxfIngestor : IDxfIngestor
    {
        public BasicDxfIngestor()
        {
        }

        public DxfDrawing ImportDrawingFromFile(string filePath)
        {
            string file = "sample.dxf";

            // create a new document, by default it will create an AutoCad2000 DXF version
            DxfDocument doc = new DxfDocument();
            // an entity
            Line entity = new Line(new Vector2(5, 5), new Vector2(10, 5));
            // add your entities here
            doc.Entities.Add(entity);
            // save to file
            doc.Save(file);

            // this check is optional but recommended before loading a DXF file
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file);
            // netDxf is only compatible with AutoCad2000 and higher DXF versions
            if (dxfVersion < DxfVersion.AutoCad2000) throw new Exception("Bad version");
            // load file
            DxfDocument loaded = DxfDocument.Load(file);

            //loaded.

            throw new NotImplementedException();
        }

        public DxfDrawing ImportDrawingFromFile(string filePath, ImportSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
