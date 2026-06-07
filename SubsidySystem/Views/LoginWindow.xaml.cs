using Microsoft.Extensions.DependencyInjection;
using SubsidySystem.Models;
using SubsidySystem.Repositories;
using SubsidySystem.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace SubsidySystem.Views
{
    public partial class LoginWindow : Window
    {
        private readonly IUserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;
        private int _loginAttempts = 0;
        private const int MAX_LOGIN_ATTEMPTS = 3;

        public LoginWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();

            Loaded += (s, e) => LoginTextBox.Focus();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await PerformLoginAsync();
        }

        private async void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await PerformLoginAsync();
            }
        }

        private async System.Threading.Tasks.Task PerformLoginAsync()
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Валидация полей
            if (string.IsNullOrEmpty(login))
            {
                ShowError("Введите логин");
                LoginTextBox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Введите пароль");
                PasswordBox.Focus();
                return;
            }

            SetControlsEnabled(false);
            ShowMessage("Проверка учетных данных...", "Gray");

            try
            {
                var user = await _userRepository.GetByLoginAsync(login);

                if (user == null)
                {
                    _loginAttempts++;
                    ShowError($"Пользователь с логином '{login}' не найден. Попытка {_loginAttempts} из {MAX_LOGIN_ATTEMPTS}");
                    ClearPassword();
                    CheckLoginAttempts();
                    return;
                }

                if (!user.IsActive)
                {
                    ShowError("Учетная запись заблокирована. Обратитесь к администратору.");
                    SetControlsEnabled(true);
                    return;
                }

                bool isValid = await _userRepository.ValidateUserAsync(login, password);

                if (!isValid)
                {
                    _loginAttempts++;
                    ShowError($"Неверный пароль. Попытка {_loginAttempts} из {MAX_LOGIN_ATTEMPTS}");
                    ClearPassword();
                    CheckLoginAttempts();
                    return;
                }

                await _userRepository.UpdateLastLoginAsync(user.UserId);

                // Сохранение информации о текущем пользователе
                UserSession.CurrentUser = new CurrentUser
                {
                    UserId = user.UserId,
                    Login = user.Login,
                    FullName = user.FullName,
                    Role = user.Role
                };

                ShowMessage("Вход выполнен успешно!", "Green");
                await System.Threading.Tasks.Task.Delay(300);

                // ОТКРЫТИЕ ГЛАВНОГО ОКНА
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();

                // ЗАКРЫТИЕ ОКНА АВТОРИЗАЦИИ
                this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка подключения к базе данных: {ex.Message}");
                MessageBox.Show($"Детали ошибки: {ex.Message}\n\n{ex.StackTrace}",
                    "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                SetControlsEnabled(true);
            }
        }

        private void CheckLoginAttempts()
        {
            if (_loginAttempts >= MAX_LOGIN_ATTEMPTS)
            {
                ShowError("Превышено количество попыток входа. Приложение будет закрыто.");
                System.Threading.Tasks.Task.Delay(2000).ContinueWith(_ =>
                {
                    Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                });
            }
            else
            {
                SetControlsEnabled(true);
            }
        }

        private void ClearPassword()
        {
            PasswordBox.Password = string.Empty;
            PasswordBox.Focus();
        }

        private void ShowError(string message)
        {
            StatusTextBlock.Text = message;
            StatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;
        }

        private void ShowMessage(string message, string color)
        {
            StatusTextBlock.Text = message;
            switch (color)
            {
                case "Red":
                    StatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                    break;
                case "Green":
                    StatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                    break;
                default:
                    StatusTextBlock.Foreground = System.Windows.Media.Brushes.Gray;
                    break;
            }
        }

        private void SetControlsEnabled(bool isEnabled)
        {
            LoginTextBox.IsEnabled = isEnabled;
            PasswordBox.IsEnabled = isEnabled;
            LoginButton.IsEnabled = isEnabled;
        }
    }
}