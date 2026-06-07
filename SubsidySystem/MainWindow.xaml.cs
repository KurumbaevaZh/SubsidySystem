using Microsoft.Extensions.DependencyInjection;
using SubsidySystem.Services;
using SubsidySystem.ViewModels;
using SubsidySystem.Views;
using System;
using System.Windows;

namespace SubsidySystem
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Проверка авторизации при загрузке окна
            if (!UserSession.IsAuthenticated)
            {
                MessageBox.Show("Сессия не активна. Пожалуйста, выполните вход.",
                    "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
                return;
            }

            // Отображение информации о пользователе
            UserNameTextBlock.Text = UserSession.CurrentUser?.FullName ?? "Неизвестный пользователь";
            UserRoleTextBlock.Text = UserSession.CurrentUser?.Role ?? "неизвестно";

            // Настройка видимости элементов в зависимости от роли
            ConfigureUIBasedOnRole();

            // Обновление статуса
            UpdateStatus($"Добро пожаловать, {UserSession.CurrentUser?.FullName}!");
        }

        /// <summary>
        /// Настройка интерфейса в зависимости от роли пользователя
        /// </summary>
        private void ConfigureUIBasedOnRole()
        {
            if (UserSession.IsAdmin)
            {
                // Администратор видит все разделы
                AdminPanel.Visibility = Visibility.Visible;
                ReportsPanel.Visibility = Visibility.Visible;
            }
            else if (UserSession.IsManager)
            {
                // Руководитель не видит настройки справочников
                AdminPanel.Visibility = Visibility.Collapsed;
                ReportsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                // Специалист видит только основные функции
                AdminPanel.Visibility = Visibility.Collapsed;
                ReportsPanel.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Обновление строки статуса
        /// </summary>
        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = message;
        }

        /// <summary>
        /// Выход из системы
        /// </summary>
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из системы?",
                "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                UserSession.Logout();

                // Открытие окна авторизации
                var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();

                this.Close();
            }
        }

        /// <summary>
        /// Открытие окна регистрации заявителя
        /// </summary>
        private void RegisterCitizen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("Открытие окна регистрации заявителя...");
                var viewModel = App.ServiceProvider.GetRequiredService<CitizenRegistrationViewModel>();
                var window = new CitizenRegistrationWindow(viewModel);
                window.Owner = this;
                window.ShowDialog();
                UpdateStatus("Готов к работе");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка при открытии окна регистрации: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Открытие окна управления членами семьи
        /// </summary>
        private void FamilyMembers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("Открытие окна управления членами семьи...");
                var viewModel = App.ServiceProvider.GetRequiredService<FamilyMemberViewModel>();
                var window = new FamilyMemberWindow(viewModel);
                window.Owner = this;
                window.ShowDialog();
                UpdateStatus("Готов к работе");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка при открытии окна: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Открытие окна ввода доходов
        /// </summary>
        private void EnterIncomes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("Открытие окна ввода доходов...");
                var viewModel = App.ServiceProvider.GetRequiredService<IncomeViewModel>();
                var window = new IncomeWindow(viewModel);
                window.Owner = this;
                window.ShowDialog();
                UpdateStatus("Готов к работе");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка при открытии окна ввода доходов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Открытие окна расчета субсидии
        /// </summary>
        private void CalculateSubsidy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("Открытие окна расчета субсидии...");
                var viewModel = App.ServiceProvider.GetRequiredService<SubsidyCalculationViewModel>();
                var window = new SubsidyCalculationWindow(viewModel);
                window.Owner = this;
                window.ShowDialog();
                UpdateStatus("Готов к работе");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка при открытии окна расчета субсидии: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Открытие окна формирования отчетов
        /// </summary>
        private void GenerateReports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatus("Открытие окна формирования отчетов...");
                var viewModel = App.ServiceProvider.GetRequiredService<ReportViewModel>();
                var window = new ReportWindow(viewModel);
                window.Owner = this;
                window.ShowDialog();
                UpdateStatus("Готов к работе");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка при открытии окна формирования отчетов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Открытие окна управления справочниками (только для администратора)
        /// </summary>
        private void ManageDictionaries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!UserSession.IsAdmin)
                {
                    MessageBox.Show("Доступ запрещен. Требуются права администратора.",
                        "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                UpdateStatus("Открытие окна управления справочниками...");
                var viewModel = App.ServiceProvider.GetRequiredService<DictionariesViewModel>();
                var window = new DictionariesWindow(viewModel);
                window.Owner = this;
                window.ShowDialog();
                UpdateStatus("Готов к работе");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Открытие окна управления пользователями (только для администратора)
        /// </summary>
        private void ManageUsers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!UserSession.IsAdmin)
                {
                    MessageBox.Show("Доступ запрещен. Требуются права администратора.",
                        "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                UpdateStatus("Открытие окна управления пользователями...");
                var viewModel = App.ServiceProvider.GetRequiredService<UsersViewModel>();
                var window = new UsersWindow(viewModel);
                window.Owner = this;
                window.ShowDialog();
                UpdateStatus("Готов к работе");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Открытие окна управления нормативами (только для администратора)
        /// </summary>
        private void ManageStandards_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!UserSession.IsAdmin)
                {
                    MessageBox.Show("Доступ запрещен. Требуются права администратора.",
                        "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                UpdateStatus("Открытие окна управления нормативами...");
                var viewModel = App.ServiceProvider.GetRequiredService<StandardsViewModel>();
                var window = new StandardsWindow(viewModel);
                window.Owner = this;
                window.ShowDialog();
                UpdateStatus("Готов к работе");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Открытие окна статистических отчетов (для руководителя и администратора)
        /// </summary>
        private void StatisticalReports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!UserSession.IsManager && !UserSession.IsAdmin)
                {
                    MessageBox.Show("Доступ запрещен. Требуются права руководителя.",
                        "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                UpdateStatus("Открытие окна статистических отчетов...");
                var viewModel = App.ServiceProvider.GetRequiredService<ReportViewModel>();
                var window = new ReportWindow(viewModel);
                window.Owner = this;
                window.ShowDialog();
                UpdateStatus("Готов к работе");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Открытие окна реестров выплат (для руководителя и администратора)
        /// </summary>
        private void PaymentRegistries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!UserSession.IsManager && !UserSession.IsAdmin)
                {
                    MessageBox.Show("Доступ запрещен. Требуются права руководителя.",
                        "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                UpdateStatus("Открытие окна реестров выплат...");
                var viewModel = App.ServiceProvider.GetRequiredService<ApproveRegistriesViewModel>();
                var window = new ApproveRegistriesWindow(viewModel);
                window.Owner = this;
                window.ShowDialog();
                UpdateStatus("Готов к работе");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}