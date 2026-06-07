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
    public class SubsidyCalculationViewModel : BaseViewModel
    {
        private readonly IRepository<Citizen> _citizenRepository;
        private readonly ISubsidyCalculationService _calculationService;

        private ObservableCollection<Citizen> _citizens = new();
        private Citizen? _selectedCitizen;
        private int _selectedYear = DateTime.Now.Year;
        private int _selectedMonth = DateTime.Now.Month;
        private ObservableCollection<int> _years = new();
        private ObservableCollection<int> _months = new();
        private bool _isCalculating;
        private string _statusMessage = string.Empty;
        private SubsidyCalculationResult? _calculationResult;

        public SubsidyCalculationViewModel(
            IRepository<Citizen> citizenRepository,
            ISubsidyCalculationService calculationService)
        {
            _citizenRepository = citizenRepository;
            _calculationService = calculationService;

            LoadCitizensCommand = new RelayCommand(async _ => await LoadCitizensAsync());
            CalculateCommand = new RelayCommand(async _ => await CalculateAsync(), _ => SelectedCitizen != null);

            InitializeYearsMonths();
            Task.Run(async () => await LoadCitizensAsync());
        }

        public ObservableCollection<Citizen> Citizens
        {
            get => _citizens;
            set => SetField(ref _citizens, value);
        }

        public Citizen? SelectedCitizen
        {
            get => _selectedCitizen;
            set => SetField(ref _selectedCitizen, value);
        }

        public int SelectedYear
        {
            get => _selectedYear;
            set => SetField(ref _selectedYear, value);
        }

        public int SelectedMonth
        {
            get => _selectedMonth;
            set => SetField(ref _selectedMonth, value);
        }

        public ObservableCollection<int> Years
        {
            get => _years;
            set => SetField(ref _years, value);
        }

        public ObservableCollection<int> Months
        {
            get => _months;
            set => SetField(ref _months, value);
        }

        public bool IsCalculating
        {
            get => _isCalculating;
            set => SetField(ref _isCalculating, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public SubsidyCalculationResult? CalculationResult
        {
            get => _calculationResult;
            set => SetField(ref _calculationResult, value);
        }

        public ICommand LoadCitizensCommand { get; }
        public ICommand CalculateCommand { get; }

        private void InitializeYearsMonths()
        {
            for (int y = DateTime.Now.Year - 2; y <= DateTime.Now.Year + 1; y++)
                Years.Add(y);

            for (int m = 1; m <= 12; m++)
                Months.Add(m);
        }

        private async Task LoadCitizensAsync()
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = "Загрузка списка граждан...";
                });

                var citizens = await _citizenRepository.GetAllAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Citizens.Clear();
                    foreach (var c in citizens.OrderBy(c => c.LastName))
                        Citizens.Add(c);

                    StatusMessage = $"Загружено {Citizens.Count} граждан";
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
        }

        private async Task CalculateAsync()
        {
            if (SelectedCitizen == null)
            {
                MessageBox.Show("Выберите заявителя", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsCalculating = true;
            StatusMessage = "Выполнение расчета...";
            CalculationResult = null;

            try
            {
                var result = await _calculationService.CalculateFullSubsidyAsync(
                    SelectedCitizen.CitizenId,
                    SelectedYear,
                    SelectedMonth);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    CalculationResult = result;
                    StatusMessage = "Расчет выполнен успешно!";

                    MessageBox.Show(
                        $"Расчет субсидии для {SelectedCitizen.LastName} {SelectedCitizen.FirstName}\n\n" +
                        $"Среднедушевой доход: {result.AveragePerCapitaIncome:N2} руб.\n" +
                        $"Прожиточный минимум: {result.LivingWage:N2} руб.\n" +
                        $"Стандарт ЖКУ: {result.HousingStandardCost:N2} руб.\n" +
                        $"Субсидия к выплате: {result.SubsidyAmount:N2} руб.",
                        "Результат расчета",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка расчета: {ex.InnerException?.Message ?? ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                IsCalculating = false;
            }
        }
    }
}