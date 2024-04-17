namespace DxfIngest
{
    /// <summary>
    /// Ingest Dxf files
    /// </summary>
    /// <remarks>
    /// Some limitations such as all linework on one layer.
    /// Potentially one layer only in drawing, or flatten, or require specific layer name
    /// Require specific linework for certain aspects of the imported drawing
    /// - To identify elements such as fixing points, holes, etc
    /// A socket needs two drawings, the drawing of the unit/faceplate, whatever,
    /// and a drawing of the holes that need to be cut.
    /// </remarks>
    public interface IDxfIngestor
    {
        public DxfDrawing ImportDrawingFromFile(string filePath);

        public DxfDrawing ImportDrawingFromFile(string filePath, ImportSettings settings);
    }
}
