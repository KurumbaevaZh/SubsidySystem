using SubsidySystem.ViewModels;
using System.Windows;

namespace SubsidySystem.Views
{
    public partial class UsersWindow : Window
    {
        public UsersWindow(UsersViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Привязка пароля
            viewModel.SetPasswordBinding(PasswordBox);
        }
    }
}