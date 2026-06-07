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
    public class StandardsViewModel : BaseViewModel
    {
        private readonly IRepository<Standard> _standardRepository;

        private ObservableCollection<Standard> _standards = new();
        private Standard _standard = new();
        private Standard? _selectedStandard;
        private ObservableCollection<string> _standardTypes = new()
        {
            "Прожиточный минимум - трудоспособный",
            "Прожиточный минимум - пенсионер",
            "Прожиточный минимум - ребенок",
            "Стандарт ЖКУ - 1 чел",
            "Стандарт ЖКУ - 2 чел",
            "Стандарт ЖКУ - 3 чел",
            "Стандарт ЖКУ - 4 чел",
            "Стандарт ЖКУ - 5 чел",
            "Максимальная доля расходов"
        };
        private bool _isLoading;
        private string _statusMessage = string.Empty;

        public StandardsViewModel(IRepository<Standard> standardRepository)
        {
            _standardRepository = standardRepository;

            LoadStandardsCommand = new RelayCommand(async _ => await LoadStandardsAsync());
            AddCommand = new RelayCommand(async _ => await AddAsync(), _ => CanSave);
            UpdateCommand = new RelayCommand(async _ => await UpdateAsync(), _ => SelectedStandard != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedStandard != null);
            ClearCommand = new RelayCommand(_ => ClearForm());

            Task.Run(async () => await LoadStandardsAsync());
        }

        public ObservableCollection<Standard> Standards
        {
            get => _standards;
            set => SetField(ref _standards, value);
        }

        public Standard Standard
        {
            get => _standard;
            set => SetField(ref _standard, value);
        }

        public Standard? SelectedStandard
        {
            get => _selectedStandard;
            set
            {
                if (SetField(ref _selectedStandard, value) && value != null)
                {
                    Standard = new Standard
                    {
                        StandardId = value.StandardId,
                        StandardType = value.StandardType,
                        StandardValue = value.StandardValue,
                        TerritoryCode = value.TerritoryCode,
                        ValidFrom = value.ValidFrom,
                        ValidTo = value.ValidTo
                    };
                }
            }
        }

        public ObservableCollection<string> StandardTypes => _standardTypes;

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

        public bool CanSave => !string.IsNullOrWhiteSpace(Standard.StandardType) &&
                                Standard.StandardValue > 0 &&
                                Standard.ValidFrom != default;

        public ICommand LoadStandardsCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

        private async Task LoadStandardsAsync()
        {
            IsLoading = true;
            try
            {
                var standards = await _standardRepository.GetAllAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Standards.Clear();
                    foreach (var s in standards.OrderBy(s => s.StandardType))
                        Standards.Add(s);
                    StatusMessage = $"Загружено {Standards.Count} нормативов";
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
                // Если дата не задана, устанавливаем текущую
                if (Standard.ValidFrom == default)
                {
                    Standard.ValidFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }

                Standard.CreatedAt = DateTime.UtcNow;

                await _standardRepository.AddAsync(Standard);
                await _standardRepository.SaveChangesAsync();

                await LoadStandardsAsync();
                ClearForm();
                StatusMessage = "Норматив добавлен!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.InnerException?.Message ?? ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task UpdateAsync()
        {
            if (SelectedStandard == null) return;

            IsLoading = true;
            try
            {
                SelectedStandard.StandardType = Standard.StandardType;
                SelectedStandard.StandardValue = Standard.StandardValue;
                SelectedStandard.TerritoryCode = Standard.TerritoryCode;
                SelectedStandard.ValidFrom = Standard.ValidFrom;
                SelectedStandard.ValidTo = Standard.ValidTo;
                SelectedStandard.UpdatedAt = DateTime.UtcNow;

                _standardRepository.Update(SelectedStandard);
                await _standardRepository.SaveChangesAsync();

                await LoadStandardsAsync();
                ClearForm();
                StatusMessage = "Норматив обновлён!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.InnerException?.Message ?? ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteAsync()
        {
            if (SelectedStandard == null) return;

            var result = MessageBox.Show($"Удалить норматив '{SelectedStandard.StandardType}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            IsLoading = true;
            try
            {
                _standardRepository.Delete(SelectedStandard);
                await _standardRepository.SaveChangesAsync();

                await LoadStandardsAsync();
                ClearForm();
                StatusMessage = "Норматив удалён!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.InnerException?.Message ?? ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearForm()
        {
            Standard = new Standard();
            SelectedStandard = null;
            StatusMessage = "Форма очищена";
        }
    }
}