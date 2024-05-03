namespace ExcelCableGeneratorApp.Dxf.Aggregates.Data;

public enum TechPanelSize
{
    NO_SIZE = 0,
    TP_1GANG = 4,
    TP_2GANG = 10,
    TP_3GANG = 16,
    TP_4GANG = 20,
    //TP_5GANG = 5,

    //TP_1GANG = 1,
    //TP_2GANG = 2,
    //TP_3GANG = 3,
    //TP_4GANG = 4,
    //TP_5GANG = 5,
}


public class TechPanelSizeInfo
{
    public static TechPanelSize SmallestPanelFromCount(int socketCount)
    {
        var sizes = Enum.GetValues(typeof(TechPanelSize));
        var sizesList = sizes.OfType<TechPanelSize>().ToList();

        // loop the panels from largest to smallest
        for (int i = sizesList.Count - 1; i > 0; i--)
        {
            var tps = sizesList[i];
            if (tps.GetSocketQuantity() > socketCount)
            {
                return tps;
            }
        }

        return TechPanelSize.NO_SIZE;
    }
}

public static class TechPanelSizeExtensions
{
    public static int GetUWidth(this TechPanelSize techPanelSize)
    {
        switch (techPanelSize)
        {
            case TechPanelSize.TP_1GANG:
                return 1;
            case TechPanelSize.TP_2GANG:
                return 2;
            case TechPanelSize.TP_3GANG:
                return 3;
            case TechPanelSize.TP_4GANG:
                return 4;
            default:
                return 0;
        }
    }

    public static int GetSocketQuantity(this TechPanelSize techPanelSize)
    {
        return (int)techPanelSize;
    }

}
