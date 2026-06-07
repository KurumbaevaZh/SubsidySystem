using SubsidySystem.Models;
using SubsidySystem.Repositories;
using SubsidySystem.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SubsidySystem.ViewModels
{
    public class ApproveRegistriesViewModel : BaseViewModel
    {
        private readonly IRepository<PaymentRegistry> _registryRepository;
        private readonly IRepository<Payment> _paymentRepository;

        private ObservableCollection<PaymentRegistry> _registries = new();
        private PaymentRegistry? _selectedRegistry;
        private ObservableCollection<Payment> _payments = new();
        private bool _isLoading;
        private string _statusMessage = string.Empty;
        private string _currentUser = UserSession.CurrentUser?.FullName ?? "Руководитель";

        public ApproveRegistriesViewModel(
            IRepository<PaymentRegistry> registryRepository,
            IRepository<Payment> paymentRepository)
        {
            _registryRepository = registryRepository;
            _paymentRepository = paymentRepository;

            LoadRegistriesCommand = new RelayCommand(async _ => await LoadRegistriesAsync());
            LoadPaymentsCommand = new RelayCommand(async _ => await LoadPaymentsAsync(), _ => SelectedRegistry != null);
            ApproveCommand = new RelayCommand(async _ => await ApproveAsync(), _ => CanApprove);
            RejectCommand = new RelayCommand(async _ => await RejectAsync(), _ => CanApprove);
            RefreshCommand = new RelayCommand(async _ => await LoadRegistriesAsync());

            Task.Run(async () => await LoadRegistriesAsync());
        }

        public ObservableCollection<PaymentRegistry> Registries
        {
            get => _registries;
            set => SetField(ref _registries, value);
        }

        public PaymentRegistry? SelectedRegistry
        {
            get => _selectedRegistry;
            set
            {
                if (SetField(ref _selectedRegistry, value))
                {
                    if (value != null)
                    {
                        LoadPaymentsCommand.Execute(null);
                    }
                    else
                    {
                        Payments.Clear();
                    }
                }
            }
        }

        public ObservableCollection<Payment> Payments
        {
            get => _payments;
            set => SetField(ref _payments, value);
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

        public bool CanApprove => SelectedRegistry != null &&
                                   SelectedRegistry.Status == "формируется";

        public ICommand LoadRegistriesCommand { get; }
        public ICommand LoadPaymentsCommand { get; }
        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }
        public ICommand RefreshCommand { get; }

        private async Task LoadRegistriesAsync()
        {
            IsLoading = true;
            StatusMessage = "Загрузка реестров...";

            try
            {
                var registries = await _registryRepository
                    .FindAsync(r => r.Status == "формируется" || r.Status == "утверждён");

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Registries.Clear();
                    foreach (var r in registries.OrderByDescending(r => r.RegistryDate))
                        Registries.Add(r);

                    StatusMessage = $"Загружено {Registries.Count} реестров";
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadPaymentsAsync()
        {
            if (SelectedRegistry == null) return;

            IsLoading = true;
            StatusMessage = "Загрузка выплат...";

            try
            {
                var payments = await _paymentRepository
                    .FindAsync(p => p.RegistryId == SelectedRegistry.RegistryId);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Payments.Clear();
                    foreach (var p in payments)
                        Payments.Add(p);

                    StatusMessage = $"Загружено {Payments.Count} выплат на сумму {SelectedRegistry.TotalAmount:N2} руб.";
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ApproveAsync()
        {
            if (SelectedRegistry == null) return;

            var result = MessageBox.Show($"Утвердить реестр №{SelectedRegistry.RegistryNumber} на сумму {SelectedRegistry.TotalAmount:N2} руб.?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            IsLoading = true;
            StatusMessage = "Утверждение реестра...";

            try
            {
                SelectedRegistry.Status = "утверждён";
                SelectedRegistry.ApprovedBy = _currentUser;
                SelectedRegistry.ApprovedDate = DateTime.Now;

                _registryRepository.Update(SelectedRegistry);
                await _registryRepository.SaveChangesAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Реестр №{SelectedRegistry.RegistryNumber} утверждён!";
                    MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                });

                await LoadRegistriesAsync();
                SelectedRegistry = null;
                Payments.Clear();
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RejectAsync()
        {
            if (SelectedRegistry == null) return;

            var result = MessageBox.Show($"Отклонить реестр №{SelectedRegistry.RegistryNumber}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            IsLoading = true;
            StatusMessage = "Отклонение реестра...";

            try
            {
                SelectedRegistry.Status = "отклонён";
                SelectedRegistry.ApprovedBy = _currentUser;
                SelectedRegistry.ApprovedDate = DateTime.Now;

                _registryRepository.Update(SelectedRegistry);
                await _registryRepository.SaveChangesAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Реестр №{SelectedRegistry.RegistryNumber} отклонён!";
                    MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                });

                await LoadRegistriesAsync();
                SelectedRegistry = null;
                Payments.Clear();
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}