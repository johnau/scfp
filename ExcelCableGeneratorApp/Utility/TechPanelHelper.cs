using ExcelCableGeneratorApp.Dxf.Aggregates.Data;

namespace ExcelCableGeneratorApp.Utility;

internal class TechPanelHelper
{

    public static List<TechPanelSize> FitToPanel(int socketQty)
    {
        List<TechPanelSize> sufficientSize = [];
        if (socketQty < (int)TechPanelSize.TP_1GANG)
        {
            sufficientSize.Add(TechPanelSize.TP_1GANG);
            sufficientSize.Add(TechPanelSize.TP_2GANG);
            sufficientSize.Add(TechPanelSize.TP_3GANG);
            sufficientSize.Add(TechPanelSize.TP_4GANG);
            //sufficientSize.Add(TechPanelSize.TP_5GANG);
        }
        else if (socketQty < (int)TechPanelSize.TP_2GANG)
        {
            sufficientSize.Add(TechPanelSize.TP_2GANG);
            sufficientSize.Add(TechPanelSize.TP_3GANG);
            sufficientSize.Add(TechPanelSize.TP_4GANG);
            //sufficientSize.Add(TechPanelSize.TP_5GANG);
        }
        else if (socketQty < (int)TechPanelSize.TP_3GANG)
        {
            sufficientSize.Add(TechPanelSize.TP_3GANG);
            sufficientSize.Add(TechPanelSize.TP_4GANG);
            //sufficientSize.Add(TechPanelSize.TP_5GANG);
        }
        else if (socketQty < (int)TechPanelSize.TP_4GANG)
        {
            sufficientSize.Add(TechPanelSize.TP_4GANG);
            //sufficientSize.Add(TechPanelSize.TP_5GANG);
        }
        //else if (socketQty < TechPanelSize.TechPanel_5Gang)
        //{
            //sufficientSize.Add(TechPanelSize.TP_5GANG);
        //}

        return sufficientSize;
    }

}
