using SubsidySystem.ViewModels;
using System.Windows;

namespace SubsidySystem.Views
{
    public partial class FamilyMemberWindow : Window
    {
        public FamilyMemberWindow(FamilyMemberViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}