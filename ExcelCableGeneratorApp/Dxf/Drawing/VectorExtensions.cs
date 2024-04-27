namespace ExcelCableGeneratorApp.Dxf.Drawing;

public static class VectorExtensions
{
    public static netDxf.Vector2 ToDxfVector2(this System.Numerics.Vector2 vec) => new netDxf.Vector2(vec.X, vec.Y);
    public static System.Numerics.Vector2 ToNumerics(this netDxf.Vector2 vec) => new System.Numerics.Vector2((float)vec.X, (float)vec.Y);
    
}
