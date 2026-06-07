using Microsoft.Extensions.DependencyInjection;
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
            viewModel.SetPasswordBinding(PasswordBox);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            this.Close();
        }
    }
}