using System.Numerics;

namespace ExcelCableGeneratorApp.Dxf.Aggregates;

internal class SectionData
{
    public Vector2 Size { get; }
    public Vector2 Position { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    /// <param name="position">Position from top left (0,0)</param>
    public SectionData(Vector2 size, Vector2 position)
    {
        Size = size;
        Position = position;
    }
}
