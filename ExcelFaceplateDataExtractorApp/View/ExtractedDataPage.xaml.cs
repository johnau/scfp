using ExcelFaceplateDataExtractorApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExcelFaceplateDataExtractorApp.View
{
    /// <summary>
    /// Interaction logic for ExtractedDataPage.xaml
    /// </summary>
    public partial class ExtractedDataPage : Page, IViewModelBehind<ExtractedDataPageViewModel>
    {
        public ExtractedDataPageViewModel ViewModel => (ExtractedDataPageViewModel) DataContext;

        public ExtractedDataPage(ExtractedDataPageViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
