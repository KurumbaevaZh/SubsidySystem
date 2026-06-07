using SubsidySystem.ViewModels;
using System.Windows;

namespace SubsidySystem.Views
{
    public partial class SubsidyCalculationWindow : Window
    {
        public SubsidyCalculationWindow(SubsidyCalculationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}