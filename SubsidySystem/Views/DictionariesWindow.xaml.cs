using SubsidySystem.ViewModels;
using System.Windows;

namespace SubsidySystem.Views
{
    public partial class DictionariesWindow : Window
    {
        public DictionariesWindow(DictionariesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}