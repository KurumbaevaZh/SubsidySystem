using Microsoft.Extensions.DependencyInjection;
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = App.ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            this.Close();
        }
    }
}