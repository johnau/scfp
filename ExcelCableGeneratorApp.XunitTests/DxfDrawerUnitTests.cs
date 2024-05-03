using ExcelCableGeneratorApp.Dxf.Aggregates;
using ExcelCableGeneratorApp.Dxf.Drawing;
using ExcelCableGeneratorApp.Dxf.Drawing.Element;
using ExcelCableGeneratorApp.Dxf.Drawing.Factory;
using netDxf;
using System.Diagnostics;

namespace ExcelCableGeneratorApp.XunitTests;

public class DxfDrawerUnitTests
{
    [Fact]
    public void DrawPanel_WithFakeData_WillSucceed()
    {
        var drawer = new BasicDxfPanelDrawer();
        //drawer.DrawPanels();
    }

    [Fact]
    public void CreateTechPanel_1GangWithDummyData_WillSucceed()
    {
        var tp = PanelFactory.BuildTechPanel_1Gang();

        // Setup sockets and get relevant index numbers
        var i1 = tp.SetupNextFreeSocketPoint("A001", SocketType.XLR_FEMALE_TYPE_A);
        var i2 = tp.SetupNextFreeSocketPoint("A002", SocketType.XLR_FEMALE_TYPE_A);
        var i3 = tp.SetupNextFreeSocketPoint("A003", SocketType.XLR_FEMALE_TYPE_A);
        var i4 = tp.SetupNextFreeSocketPoint("A004", SocketType.XLR_FEMALE_TYPE_A);

        var group1 = "ER400";
        var group2 = "ER401";

        tp.CreateSocketGroup(group1);
        tp.CreateSocketGroup(group2);

        tp.AddSocketIndexToGroup(i1, group1);
        tp.AddSocketIndexToGroup(i2, group1);
        tp.AddSocketIndexToGroup(i3, group2);
        tp.AddSocketIndexToGroup(i4, group2);

        DxfDocument doc = new();
        
        tp.Draw(doc.Entities);
        doc.Save("./tp_test_1gang.dxf");
    }

    [Fact]
    public void CreateTechPanel_2GangWithDummyData_WillSucceed()
    {
        var tp = PanelFactory.BuildTechPanel_2Gang();

        // Setup sockets and get relevant index numbers
        var i1 = tp.SetupNextFreeSocketPoint("A001", SocketType.XLR_FEMALE_TYPE_A);
        var i2 = tp.SetupNextFreeSocketPoint("A002", SocketType.XLR_FEMALE_TYPE_A);
        var i3 = tp.SetupNextFreeSocketPoint("A003", SocketType.XLR_FEMALE_TYPE_A);
        var i4 = tp.SetupNextFreeSocketPoint("A004", SocketType.XLR_FEMALE_TYPE_A);

        var group1 = "ER400";
        var group2 = "ER401";

        tp.CreateSocketGroup(group1);
        tp.CreateSocketGroup(group2);

        tp.AddSocketIndexToGroup(i1, group1);
        tp.AddSocketIndexToGroup(i2, group1);
        tp.AddSocketIndexToGroup(i3, group2);
        tp.AddSocketIndexToGroup(i4, group2);

        DxfDocument doc = new();

        tp.Draw(doc.Entities);
        doc.Save("./tp_test_2gang.dxf");
    }

    [Fact]
    public void CreateTechPanel_3GangWithDummyData_WillSucceed()
    {
        var tp = PanelFactory.BuildTechPanel_3Gang();

        // Setup sockets and get relevant index numbers
        var i1 = tp.SetupNextFreeSocketPoint("A001", SocketType.XLR_FEMALE_TYPE_A);
        var i2 = tp.SetupNextFreeSocketPoint("A002", SocketType.XLR_FEMALE_TYPE_A);
        var i3 = tp.SetupNextFreeSocketPoint("A003", SocketType.XLR_FEMALE_TYPE_A);
        var i4 = tp.SetupNextFreeSocketPoint("A004", SocketType.XLR_FEMALE_TYPE_A);

        var group1 = "ER400";
        var group2 = "ER401";

        tp.CreateSocketGroup(group1);
        tp.CreateSocketGroup(group2);

        tp.AddSocketIndexToGroup(i1, group1);
        tp.AddSocketIndexToGroup(i2, group1);
        tp.AddSocketIndexToGroup(i3, group2);
        tp.AddSocketIndexToGroup(i4, group2);

        DxfDocument doc = new();

        tp.Draw(doc.Entities);
        doc.Save("./tp_test_3gang.dxf");
    }

    [Fact]
    public void CreateTechPanel_4GangWithDummyData_WillSucceed()
    {
        var tp = PanelFactory.BuildTechPanel_4Gang();

        // Setup sockets and get relevant index numbers
        var i1 = tp.SetupNextFreeSocketPoint("A001", SocketType.XLR_FEMALE_TYPE_A);
        var i2 = tp.SetupNextFreeSocketPoint("A002", SocketType.XLR_FEMALE_TYPE_A);
        var i3 = tp.SetupNextFreeSocketPoint("A003", SocketType.XLR_FEMALE_TYPE_A);
        var i4 = tp.SetupNextFreeSocketPoint("A004", SocketType.XLR_FEMALE_TYPE_A);

        var group1 = "ER400";
        var group2 = "ER401";

        tp.CreateSocketGroup(group1);
        tp.CreateSocketGroup(group2);

        tp.AddSocketIndexToGroup(i1, group1);
        tp.AddSocketIndexToGroup(i2, group1);
        tp.AddSocketIndexToGroup(i3, group2);
        tp.AddSocketIndexToGroup(i4, group2);

        DxfDocument doc = new();

        tp.Draw(doc.Entities);
        doc.Save("./tp_test_4gang.dxf");
    }

    [Fact]
    public void CreateTechPanel_5GangWithDummyData_WillSucceed()
    {
        var tp = PanelFactory.BuildTechPanel_5Gang();

        // Setup sockets and get relevant index numbers
        var i1 = tp.SetupNextFreeSocketPoint("A001", SocketType.XLR_FEMALE_TYPE_A);
        var i2 = tp.SetupNextFreeSocketPoint("A002", SocketType.XLR_FEMALE_TYPE_A);
        var i3 = tp.SetupNextFreeSocketPoint("A003", SocketType.XLR_FEMALE_TYPE_A);
        var i4 = tp.SetupNextFreeSocketPoint("A004", SocketType.XLR_FEMALE_TYPE_A);

        var group1 = "ER400";
        var group2 = "ER401";

        tp.CreateSocketGroup(group1);
        tp.CreateSocketGroup(group2);

        tp.AddSocketIndexToGroup(i1, group1);
        tp.AddSocketIndexToGroup(i2, group1);
        tp.AddSocketIndexToGroup(i3, group2);
        tp.AddSocketIndexToGroup(i4, group2);

        DxfDocument doc = new();

        tp.Draw(doc.Entities);
        doc.Save("./tp_test_5gang.dxf");
    }

    [Fact]
    public void CreateDrawingObjects_WithDummyData_WillSuceed()
    {
        var panelDraw = new BasicDxfPanelDrawer();
        var panel = panelDraw.BuildPanel(0); // lazy - exposed for easier testing 

        var testSocket1 = new SocketData("A001", SocketType.XLR_FEMALE_TYPE_A);
        var testSocket2 = new SocketData("A002", SocketType.XLR_FEMALE_TYPE_A);
        var testSocket3 = new SocketData("A003", SocketType.XLR_MALE_TYPE_A);
        var testSocket4 = new SocketData("A004", SocketType.XLR_MALE_TYPE_A);

        var testGroup1 = new SocketGroupData("ER401", "AUDIO SEND/RETURN", "Belden 108XX", [testSocket1, testSocket2, testSocket3, testSocket4]);

        var testSocket5 = new SocketData("DM105", SocketType.DMX_FEMALE_TYPE_D);
        var testSocket6 = new SocketData("DM106", SocketType.DMX_FEMALE_TYPE_D);
        var testSocket7 = new SocketData("DM107", SocketType.DMX_FEMALE_TYPE_D);

        var testGroup2 = new SocketGroupData("ER211", "DMX", "Cat6a?", [testSocket5, testSocket6, testSocket7]);

        var testSocket8 = new SocketData("DM108", SocketType.DMX_FEMALE_TYPE_D);
        var testSocket9 = new SocketData("DM109", SocketType.DMX_FEMALE_TYPE_D);
        var testSocket10 = new SocketData("DM110", SocketType.DMX_FEMALE_TYPE_D);
        var testSocket11 = new SocketData("DM111", SocketType.DMX_FEMALE_TYPE_D);
        var testSocket12 = new SocketData("DM112", SocketType.DMX_FEMALE_TYPE_D);

        var testGroup3 = new SocketGroupData("ER402", "DMX", "Catg6a?", [testSocket8, testSocket9, testSocket10, testSocket11, testSocket12]);

        var socketGroup1 = panelDraw.CreateSocketGroup(testGroup1);
        var socketGroup2 = panelDraw.CreateSocketGroup(testGroup2);
        var socketGroup3 = panelDraw.CreateSocketGroup(testGroup3);

        panel.TryAddSocketGroup(socketGroup1);
        panel.TryAddSocketGroup(socketGroup2);
        panel.TryAddSocketGroup(socketGroup3);

        DxfDocument doc = new DxfDocument();
        
        panel.Draw(doc.Entities);
        doc.Save("./panel_test.dxf");


        //Assert.Equal(4, socketGroup1.Sockets.Count);
        //Assert.Equal(104f, socketGroup1.Size.X);
        //Assert.Equal(31f, socketGroup1.Size.Y);

        //Assert.Equal(3, socketGroup2.Sockets.Count);
        //Assert.Equal(78f, socketGroup2.Size.X);
        //Assert.Equal(31f, socketGroup2.Size.Y);

        PrintSocketGroup(socketGroup1);
        PrintSocketGroup(socketGroup2);
    }

    private void PrintSocketGroup(SocketGroup groupDrawingObject)
    {
        var groupId = groupDrawingObject.Id;
        var groupNameTag = groupDrawingObject.NameTag;
        var groupPosition = groupDrawingObject.Position;
        var groupSize = groupDrawingObject.Size;
        var socketsInGroup = groupDrawingObject.Sockets;
        var groupLabel = groupDrawingObject.GetBracketLabelText();
        Debug.WriteLine($"Socket Group: [id: {groupId}, nt: {groupNameTag}, pos: ({groupPosition.X}, {groupPosition.Y}), size: ({groupSize.X}, {groupSize.Y}), socket count: {socketsInGroup.Count}, label: {groupLabel}]");

        foreach (var socket in socketsInGroup) {
            var socketId = socket.Id;
            var socketNameTag = socket.NameTag;
            var socketPosition = socket.Position;
            var socketSize = socket.Size;
            var socketType = socket.SocketType;
            var socketLabel = socket.GetLabelText();
            Debug.WriteLine($"Socket: [id: {socketId}, nt: {socketNameTag}, pos: {socketPosition}, size: {socketSize}, typ: {socketType}, label: {socketLabel}]");
        }
    }
}
