using FaceplateDataExtractor.Excel;
using FaceplateDataExtractor.Model;
using FaceplateDataExtractor.Model.Mapper;
using System.Diagnostics;
using System.Reflection;
using static FaceplateDataExtractor.Excel.ControlledMsExcelFaceplateDataExtractor;

namespace FaceplateDataExtractor.XunitTests
{
    public class UnitTestExcelDataExtractor
    {
        [Fact]
        public void TestExtract()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resourceFolderPath = Path.Combine(assemblyDirectory!, "resources");
            var fileName = "20200320 - 1010 (0) CABLE SCHEDULE ken and marty6.xlsx";
            var filePath = Path.Combine(resourceFolderPath, fileName);

            var extractor = new MsExcelFaceplateDataExtractor(filePath, 1);

            Debug.WriteLine(filePath);
            var exists = File.Exists(filePath);
            Assert.True(exists);

            if (!extractor.TryExtractData(0, out var data, out var rejectedData))
            {
                Debug.WriteLine("Unable to extract data");
                return;
            }

            foreach (var d in data)
            {
                Debug.WriteLine($"EXTRACTED DATA:: {d}");
            }
        }

        [Fact]
        public void ControlledDataExtractor_WithDefaultSettings_WillSucceed()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resourceFolderPath = Path.Combine(assemblyDirectory!, "resources");
            var fileName = "20200320 - 1010 (0) CABLE SCHEDULE ken and marty6.xlsx";
            var filePath = Path.Combine(resourceFolderPath, fileName);

            var extractor = DataExtractorFactory.CreateWithMasterTemplateFixedLayout(filePath);
            var success = extractor.TryExtractData(0, out var data, out var rejectedData);
            Assert.True(success);

            foreach (var d in data)
            {
                Debug.WriteLine($"Entry: PanelId={d.PanelId} | Description={d.Description} | Location={d.Location} | Room={d.Room} | AFFL={d.AboveFinishedFloorLevel}");

                foreach (var system in d.CableSystemDatas)
                {
                    //Debug.WriteLine($"SYSTEM: {system.SystemType} {system.CableType} {system.Quantity} {system.Destination}");
                    Debug.WriteLine($"SYSTEM: {system.SystemType} | {system.Quantity} | {system.DestPanelId}");
                }
            }
        }

        [Fact]
        public void ControlledDataExtractor_WithManualSettings_WillSucceed()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resourceFolderPath = Path.Combine(assemblyDirectory!, "resources");
            var fileName = "20200320 - 1010 (0) CABLE SCHEDULE ken and marty6.xlsx";
            var filePath = Path.Combine(resourceFolderPath, fileName);

            var dataStartRow = 6;
            var dataEndRow = 200;
            var panelIdLayout = new ColumnLayout(PanelDescriptorDataType.PANEL_ID.ToString(), 1, 4, 4);
            var descriptionLayout = new ColumnLayout(PanelDescriptorDataType.DESCRIPTION.ToString(), 2, 4, 4);
            var locationLayout = new ColumnLayout(PanelDescriptorDataType.LOCATION.ToString(), 3, 4, 4);
            var roomLayout = new ColumnLayout(PanelDescriptorDataType.ROOM.ToString(), 4, 4, 4);
            var afflLayout = new ColumnLayout(PanelDescriptorDataType.AFFL.ToString(), 5, 4, 4);

            // grouped columns (system data)
            var technicalDataQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 6, 2, 4);
            var technicalDataDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 7, 2, 4);
            var technicalDataGroup = new ColumnSet(
                "TECHNICAL DATA", 
                [ technicalDataQtyLayout, technicalDataDestLayout ]
            );

            var mmFiberQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 8, 2, 4);
            var mmFiberDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 9, 2, 4);
            var mmDataGroup = new ColumnSet(
                "MULTIMODE FIBER",
                [mmFiberQtyLayout, mmFiberDestLayout]
            );

            var vtlQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 10, 2, 4);
            var vtlDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 11, 2, 4);
            var vtlDataGroup = new ColumnSet(
                "VIDEO TIE LINE",
                [vtlQtyLayout, vtlDestLayout]
            );

            var digMediaQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 12, 2, 4);
            var digMediaDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 13, 2, 4);
            var digMediaGroup = new ColumnSet(
                "DIGITAL MEDIA",
                [digMediaQtyLayout, digMediaDestLayout]
            );

            var avControlQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY.ToString(), 14, 2, 4);
            var avControlDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 15, 2, 4);
            var avControlGroup = new ColumnSet(
                "AV CONTROL",
                [avControlQtyLayout, avControlDestLayout]
            );

            var audioDigiAnlgSendQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY_MALE.ToString(), 16, 2, 4);
            var audioDigiAnlgSendDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 18, 2, 4);
            var audioSendGroup = new ColumnSet(
                "AUDIO DIGITAL/ANALOG SEND",
                [audioDigiAnlgSendQtyLayout, audioDigiAnlgSendDestLayout]
            );

            var audioDigiAnlgReturnQtyLayout = new ColumnLayout(ColumnValueType.QUANTITY_FEMALE.ToString(), 17, 2, 4);
            var audioDigiAnlgReturnDestLayout = new ColumnLayout(ColumnValueType.TO_FROM.ToString(), 18, 2, 4);
            var audioReturnGroup = new ColumnSet(
                "AUDIO DIGITAL/ANALOG RETURN"    ,
                [audioDigiAnlgReturnQtyLayout, audioDigiAnlgReturnDestLayout]
            );

            var columns = new Dictionary<string, ColumnLayout>();
            columns[panelIdLayout.Name] = panelIdLayout;
            columns[descriptionLayout.Name] = descriptionLayout;
            columns[locationLayout.Name] = locationLayout;
            columns[roomLayout.Name] = roomLayout;
            columns[afflLayout.Name] = afflLayout;

            var columnGroups = new Dictionary<string, ColumnSet>() {
                {technicalDataGroup.Name, technicalDataGroup },
                {mmDataGroup.Name, mmDataGroup },
                {vtlDataGroup.Name, vtlDataGroup },
                {digMediaGroup.Name, digMediaGroup },
                {avControlGroup.Name, avControlGroup },
                {audioSendGroup.Name, audioSendGroup },
                {audioReturnGroup.Name, audioReturnGroup}
            };

            var config = new Configuration(dataStartRow, dataEndRow, columns, columnGroups);
            var extractor = new ControlledMsExcelFaceplateDataExtractor(filePath, 1, config);
            var success = extractor.TryExtractData(0, out var data, out var rejectedData);
            Assert.True(success);

            foreach (var d in data)
            {
                Debug.WriteLine($"Entry: PanelId={d.PanelId} | Description={d.Description} | Location={d.Location} | Room={d.Room} | AFFL={d.AboveFinishedFloorLevel}");

                foreach (var system in d.CableSystemDatas)
                {
                    //Debug.WriteLine($"SYSTEM: {system.SystemType} {system.CableType} {system.Quantity} {system.Destination}");
                    Debug.WriteLine($"SYSTEM: {system.SystemType} | {system.Quantity} | {system.DestPanelId}");
                }
            }
        }
    }
}