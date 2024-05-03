using ExcelCableGeneratorApp.Dxf.Drawing.Element;
using netDxf;
using netDxf.Collections;
using netDxf.Entities;

namespace ExcelCableGeneratorApp.Dxf.Drawing.Helper;

/// <summary>
/// Not used at the moment, using the Socket object to draw itself
/// </summary>
internal class SocketDrawerHelper
{

    public static void DrawSocket(DrawingEntities drawing, SocketType socketType, Vector2 position, bool posIsCenter = true)
    {
        switch (socketType)
        {
            case SocketType.NONE:
                break;
            case SocketType.XLR_FEMALE_TYPE_A:
                DrawSocket_XLR_Female_TypeA(drawing, position, posIsCenter);
                break;
            case SocketType.XLR_MALE_TYPE_A:
                break;
            case SocketType.XLR_FEMALE_TYPE_D:
                break;
            case SocketType.XLR_MALE_TYPE_D:
                break;
            case SocketType.DMX_FEMALE_TYPE_A:
                break;
            case SocketType.DMX_MALE_TYPE_A:
                break;
            case SocketType.DMX_FEMALE_TYPE_D:
                break;
            case SocketType.DMX_MALE_TYPE_D:
                break;
            case SocketType.CAT6A_FEMALE_TYPE_D:
                break;
            default:
                break;
        }
    }

    public static void DrawSocket_XLR_Female_TypeA(DrawingEntities drawing, Vector2 position, bool posIsCenter = true)
    {
        if (posIsCenter)
        {
            var socketHole = new Circle(position, 11.0d);
            drawing.Add(socketHole);
        }
        else
            throw new NotImplementedException();
    }

}
