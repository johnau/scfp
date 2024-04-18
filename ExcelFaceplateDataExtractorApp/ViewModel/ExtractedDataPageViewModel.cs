using CommunityToolkit.Mvvm.ComponentModel;

namespace ExcelFaceplateDataExtractorApp.ViewModel;

public partial class ExtractedDataPageViewModel : ObservableObject
{
    [ObservableProperty]
    public List<string> _extractedData = []; // need to sort out the type for this
    [ObservableProperty]
    public string _fileName = string.Empty;

    [ObservableProperty]
    public string _filePath = string.Empty;

    [ObservableProperty]
    public int _numberOfEntries = 0;

}
