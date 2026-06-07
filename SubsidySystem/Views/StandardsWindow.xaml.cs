using SubsidySystem.ViewModels;
using System.Windows;

namespace SubsidySystem.Views
{
    public partial class StandardsWindow : Window
    {
        public StandardsWindow(StandardsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}