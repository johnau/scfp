using ExcelCableGeneratorApp.Dxf.Aggregates.Data;
using ExcelCableGeneratorApp.Dxf.Drawing.Element;
using ExcelCableGeneratorApp.Dxf.Drawing.Factory;

namespace ExcelCableGeneratorApp.Dxf;

internal class TechPanelBuilder
{

    public FixedGridPanel BuildFixedGridPanel(TechPanelSize size, string panelId, List<SystemGroupContents> groupsOnPanel)
    {
        FixedGridPanel panel;
        switch (size)
        {
            case TechPanelSize.TP_1GANG:
                panel = PanelFactory.BuildTechPanel_1Gang();
                break;
            case TechPanelSize.TP_2GANG:
                panel = PanelFactory.BuildTechPanel_2Gang();
                break;
            case TechPanelSize.TP_3GANG:
                panel = PanelFactory.BuildTechPanel_3Gang();
                break;
            case TechPanelSize.TP_4GANG:
                panel = PanelFactory.BuildTechPanel_4Gang();
                break;
            //case TechPanelSize.TP_5GANG:
            //    panel = PanelFactory.BuildTechPanel_5Gang();
            //    break;
            default:
                throw new Exception("Panel size is not supported: " + size.ToString());
        }

        panel.SetLabelText(panelId);

        foreach (var group in groupsOnPanel)
        {
            var sysName = group.SystemName;
            if (sysName.Length > 10)
                sysName = sysName.Substring(0, 10);

            panel.CreateSocketGroup(sysName);

            foreach (var socket in group.Sockets)
            {
                var s = panel.SetupNextFreeSocketPoint(socket.Key, SocketType.NONE, socket.Value);
                panel.AddSocketIndexToGroup(s, sysName);
            }
        }

        return panel;
    }


}
