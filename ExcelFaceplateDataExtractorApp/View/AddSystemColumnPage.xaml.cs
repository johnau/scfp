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
    /// Interaction logic for AddSystemColumnPage.xaml
    /// </summary>
    public partial class AddSystemColumnPage : Page, IViewModelBehind<AddSystemColumnViewModel>
    {
        public AddSystemColumnViewModel ViewModel => (AddSystemColumnViewModel) DataContext;
        public AddSystemColumnPage(AddSystemColumnViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }


    }
}
