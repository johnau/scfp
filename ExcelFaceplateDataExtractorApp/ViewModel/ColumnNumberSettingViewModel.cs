
using CommunityToolkit.Mvvm.ComponentModel;

namespace ExcelFaceplateDataExtractorApp.ViewModel;

public partial class ColumnNumberSettingViewModel : ObservableObject
{
    [ObservableProperty]
    public string _settingName = string.Empty;
    [ObservableProperty]
    public int _columnNumber;
    [ObservableProperty]
    public int _dataStartRow;

}
