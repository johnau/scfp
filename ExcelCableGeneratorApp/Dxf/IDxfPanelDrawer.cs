using ExcelCableGeneratorApp.Dxf.Aggregates;
using ExcelCableGeneratorApp.Dxf.Drawing.Element;
using ExcelCableGeneratorApp.Identifier.Aggregates;

namespace ExcelCableGeneratorApp.Dxf;

internal interface IDxfPanelDrawer
{
    /// <summary>
    /// Draws all panels for a group and returns the file path for the created file
    /// </summary>
    /// <param name="rackCableGroup"></param>
    /// <returns>string FilePath of the created DXF file</returns>
    string DrawPanels(List<SocketGroup> socketGroups);
}
