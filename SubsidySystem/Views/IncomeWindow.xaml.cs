using SubsidySystem.ViewModels;
using System.Windows;

namespace SubsidySystem.Views
{
    public partial class IncomeWindow : Window
    {
        public IncomeWindow(IncomeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}