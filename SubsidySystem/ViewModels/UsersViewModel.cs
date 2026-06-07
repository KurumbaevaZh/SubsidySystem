using SubsidySystem.Models;
using SubsidySystem.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SubsidySystem.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        private readonly IRepository<User> _userRepository;

        private ObservableCollection<User> _users = new();
        private User _user = new();
        private User? _selectedUser;
        private ObservableCollection<string> _roles = new() { "специалист", "руководитель", "администратор" };
        private bool _isLoading;
        private string _statusMessage = string.Empty;

        public UsersViewModel(IRepository<User> userRepository)
        {
            _userRepository = userRepository;

            LoadUsersCommand = new RelayCommand(async _ => await LoadUsersAsync());
            AddCommand = new RelayCommand(async _ => await AddAsync(), _ => CanSave);
            UpdateCommand = new RelayCommand(async _ => await UpdateAsync(), _ => SelectedUser != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedUser != null);
            ClearCommand = new RelayCommand(_ => ClearForm());
            ResetPasswordCommand = new RelayCommand(async _ => await ResetPasswordAsync(), _ => SelectedUser != null);

            Task.Run(async () => await LoadUsersAsync());
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetField(ref _users, value);
        }

        public User User
        {
            get => _user;
            set => SetField(ref _user, value);
        }

        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetField(ref _selectedUser, value) && value != null)
                {
                    User = new User
                    {
                        UserId = value.UserId,
                        Login = value.Login,
                        PasswordHash = "",
                        FullName = value.FullName,
                        Role = value.Role,
                        IsActive = value.IsActive
                    };
                }
            }
        }

        public ObservableCollection<string> Roles
        {
            get => _roles;
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public bool CanSave => !string.IsNullOrWhiteSpace(User.Login) &&
                                !string.IsNullOrWhiteSpace(User.PasswordHash) &&
                                !string.IsNullOrWhiteSpace(User.FullName) &&
                                !string.IsNullOrWhiteSpace(User.Role);

        public ICommand LoadUsersCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ResetPasswordCommand { get; }

        private async Task LoadUsersAsync()
        {
            IsLoading = true;
            try
            {
                var users = await _userRepository.GetAllAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Users.Clear();
                    foreach (var u in users.OrderBy(u => u.Login))
                        Users.Add(u);
                    StatusMessage = $"Загружено {Users.Count} пользователей";
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AddAsync()
        {
            if (!CanSave) return;

            IsLoading = true;
            try
            {
                var newUser = new User
                {
                    Login = User.Login,
                    PasswordHash = User.PasswordHash,
                    FullName = User.FullName,
                    Role = User.Role,
                    IsActive = User.IsActive,
                    CreatedAt = DateTime.Now
                };

                await _userRepository.AddAsync(newUser);
                await _userRepository.SaveChangesAsync();

                await LoadUsersAsync();
                ClearForm();
                StatusMessage = "Пользователь добавлен!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task UpdateAsync()
        {
            if (SelectedUser == null) return;

            IsLoading = true;
            try
            {
                SelectedUser.Login = User.Login;
                SelectedUser.FullName = User.FullName;
                SelectedUser.Role = User.Role;
                SelectedUser.IsActive = User.IsActive;

                _userRepository.Update(SelectedUser);
                await _userRepository.SaveChangesAsync();

                await LoadUsersAsync();
                ClearForm();
                StatusMessage = "Данные пользователя обновлены!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteAsync()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show($"Удалить пользователя '{SelectedUser.Login}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            IsLoading = true;
            try
            {
                _userRepository.Delete(SelectedUser);
                await _userRepository.SaveChangesAsync();

                await LoadUsersAsync();
                ClearForm();
                StatusMessage = "Пользователь удалён!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ResetPasswordAsync()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show($"Сбросить пароль для пользователя '{SelectedUser.Login}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                SelectedUser.PasswordHash = "123456";
                _userRepository.Update(SelectedUser);
                await _userRepository.SaveChangesAsync();
                StatusMessage = $"Пароль для {SelectedUser.Login} сброшен на '123456'!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
        }
        // Добавьте в класс UsersViewModel:
        private System.Windows.Controls.PasswordBox? _passwordBox;

        public void SetPasswordBinding(System.Windows.Controls.PasswordBox passwordBox)
        {
            _passwordBox = passwordBox;
            _passwordBox.PasswordChanged += (s, e) =>
            {
                User.PasswordHash = _passwordBox.Password;
            };

            // Обновление при выборе пользователя
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedUser) && SelectedUser != null)
                {
                    _passwordBox.Password = "";
                }
            };
        }

        private void ClearForm()
        {
            User = new User();
            SelectedUser = null;
            StatusMessage = "Форма очищена";
        }
    }
}