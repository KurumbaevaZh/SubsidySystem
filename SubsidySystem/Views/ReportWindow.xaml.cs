using SubsidySystem.ViewModels;
using System.Windows;

namespace SubsidySystem.Views
{
    public partial class ReportWindow : Window
    {
        public ReportWindow(ReportViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}