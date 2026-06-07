using SubsidySystem.ViewModels;
using System.Windows;

namespace SubsidySystem.Views
{
    public partial class ApproveRegistriesWindow : Window
    {
        public ApproveRegistriesWindow(ApproveRegistriesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}