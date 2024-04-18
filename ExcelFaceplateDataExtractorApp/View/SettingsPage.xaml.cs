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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page, IViewModelBehind<SettingsPageViewModel>
    {
        public SettingsPageViewModel ViewModel => (SettingsPageViewModel)DataContext;

        public SettingsPage(SettingsPageViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
