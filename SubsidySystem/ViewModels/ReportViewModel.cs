using SubsidySystem.Models;
using SubsidySystem.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SubsidySystem.ViewModels
{
    public class ReportViewModel : BaseViewModel
    {
        private readonly IRepository<SubsidyCalculation> _subsidyRepository;
        private readonly IRepository<CompensationCalculation> _compensationRepository;
        private readonly IRepository<Citizen> _citizenRepository;
        private readonly IRepository<Payment> _paymentRepository;

        private int _selectedYear = DateTime.Now.Year;
        private int _selectedMonth = DateTime.Now.Month;
        private ObservableCollection<int> _years = new();
        private ObservableCollection<int> _months = new();
        private string _selectedReportType = "Субсидии";
        private ObservableCollection<string> _reportTypes = new();
        private DataTable? _reportData;
        private bool _isLoading;
        private string _statusMessage = string.Empty;

        public ReportViewModel(
            IRepository<SubsidyCalculation> subsidyRepository,
            IRepository<CompensationCalculation> compensationRepository,
            IRepository<Citizen> citizenRepository,
            IRepository<Payment> paymentRepository)
        {
            _subsidyRepository = subsidyRepository;
            _compensationRepository = compensationRepository;
            _citizenRepository = citizenRepository;
            _paymentRepository = paymentRepository;

            GenerateReportCommand = new RelayCommand(async _ => await GenerateReportAsync());
            ExportExcelCommand = new RelayCommand(_ => ExportToExcel(), _ => ReportData != null && ReportData.Rows.Count > 0);

            InitializeYearsMonths();
            InitializeReportTypes();
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

        public string SelectedReportType
        {
            get => _selectedReportType;
            set => SetField(ref _selectedReportType, value);
        }

        public ObservableCollection<string> ReportTypes
        {
            get => _reportTypes;
            set => SetField(ref _reportTypes, value);
        }

        public DataTable? ReportData
        {
            get => _reportData;
            set => SetField(ref _reportData, value);
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

        public ICommand GenerateReportCommand { get; }
        public ICommand ExportExcelCommand { get; }

        private void InitializeYearsMonths()
        {
            for (int y = DateTime.Now.Year - 2; y <= DateTime.Now.Year + 1; y++)
                Years.Add(y);

            for (int m = 1; m <= 12; m++)
                Months.Add(m);
        }

        private void InitializeReportTypes()
        {
            ReportTypes.Add("Субсидии");
            ReportTypes.Add("Компенсации");
            ReportTypes.Add("Выплаты");
            ReportTypes.Add("Сводный отчет");
        }

        private async Task GenerateReportAsync()
        {
            IsLoading = true;
            StatusMessage = "Формирование отчета...";
            ReportData = null;

            try
            {
                switch (SelectedReportType)
                {
                    case "Субсидии":
                        await GenerateSubsidyReportAsync();
                        break;
                    case "Компенсации":
                        await GenerateCompensationReportAsync();
                        break;
                    case "Выплаты":
                        await GeneratePaymentsReportAsync();
                        break;
                    case "Сводный отчет":
                        await GenerateSummaryReportAsync();
                        break;
                }

                StatusMessage = $"Отчет сформирован. Записей: {ReportData?.Rows.Count ?? 0}";
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

        private async Task GenerateSubsidyReportAsync()
        {
            var calculations = await _subsidyRepository
                .FindAsync(s => s.Year == SelectedYear && s.Month == SelectedMonth);

            var table = new DataTable();
            table.Columns.Add("ФИО", typeof(string));
            table.Columns.Add("Совокупный доход", typeof(decimal));
            table.Columns.Add("Среднедушевой доход", typeof(decimal));
            table.Columns.Add("Прожиточный минимум", typeof(decimal));
            table.Columns.Add("Поправочный коэффициент", typeof(decimal));
            table.Columns.Add("Стандарт ЖКУ", typeof(decimal));
            table.Columns.Add("Сумма субсидии", typeof(decimal));
            table.Columns.Add("Статус", typeof(string));

            foreach (var calc in calculations)
            {
                var citizen = await _citizenRepository.GetByIdAsync(calc.CitizenId);
                table.Rows.Add(
                    citizen?.LastName + " " + citizen?.FirstName,
                    calc.TotalFamilyIncome,
                    calc.AveragePerCapitaIncome,
                    calc.LivingWage,
                    calc.CorrectionFactor,
                    calc.HousingStandard,
                    calc.SubsidyAmount,
                    calc.Status);
            }

            ReportData = table;
        }

        private async Task GenerateCompensationReportAsync()
        {
            var calculations = await _compensationRepository
                .FindAsync(c => c.Year == SelectedYear && c.Month == SelectedMonth);

            var table = new DataTable();
            table.Columns.Add("ФИО", typeof(string));
            table.Columns.Add("Категория", typeof(string));
            table.Columns.Add("Скидка %", typeof(decimal));
            table.Columns.Add("Начисления ЖКУ", typeof(decimal));
            table.Columns.Add("Сумма компенсации", typeof(decimal));
            table.Columns.Add("Статус", typeof(string));

            foreach (var calc in calculations)
            {
                var citizen = await _citizenRepository.GetByIdAsync(calc.CitizenId);
                table.Rows.Add(
                    citizen?.LastName + " " + citizen?.FirstName,
                    calc.CategoryId,
                    calc.DiscountRate,
                    calc.ActualCharges,
                    calc.CompensationAmount,
                    calc.Status);
            }

            ReportData = table;
        }

        private async Task GeneratePaymentsReportAsync()
        {
            var payments = await _paymentRepository
                .FindAsync(p => p.PaymentDate.HasValue &&
                                p.PaymentDate.Value.Year == SelectedYear &&
                                p.PaymentDate.Value.Month == SelectedMonth);

            var table = new DataTable();
            table.Columns.Add("ФИО", typeof(string));
            table.Columns.Add("Тип выплаты", typeof(string));
            table.Columns.Add("Сумма", typeof(decimal));
            table.Columns.Add("Способ выплаты", typeof(string));
            table.Columns.Add("Дата выплаты", typeof(DateTime));
            table.Columns.Add("Статус", typeof(string));

            foreach (var payment in payments)
            {
                var citizen = await _citizenRepository.GetByIdAsync(payment.CitizenId);
                table.Rows.Add(
                    citizen?.LastName + " " + citizen?.FirstName,
                    payment.PaymentType,
                    payment.PaymentAmount,
                    payment.PaymentMethod,
                    payment.PaymentDate,
                    payment.Status);
            }

            ReportData = table;
        }

        private async Task GenerateSummaryReportAsync()
        {
            var table = new DataTable();
            table.Columns.Add("Показатель", typeof(string));
            table.Columns.Add("Значение", typeof(string));

            var subsidies = await _subsidyRepository
                .FindAsync(s => s.Year == SelectedYear && s.Month == SelectedMonth);
            var compensations = await _compensationRepository
                .FindAsync(c => c.Year == SelectedYear && c.Month == SelectedMonth);

            table.Rows.Add("Количество субсидий", subsidies.Count());
            table.Rows.Add("Общая сумма субсидий", subsidies.Sum(s => s.SubsidyAmount).ToString("N2") + " руб.");
            table.Rows.Add("Средняя сумма субсидии", (subsidies.Any() ? subsidies.Average(s => s.SubsidyAmount).ToString("N2") : "0") + " руб.");
            table.Rows.Add("Количество компенсаций", compensations.Count());
            table.Rows.Add("Общая сумма компенсаций", compensations.Sum(c => c.CompensationAmount).ToString("N2") + " руб.");
            table.Rows.Add("Средняя сумма компенсации", (compensations.Any() ? compensations.Average(c => c.CompensationAmount).ToString("N2") : "0") + " руб.");
            table.Rows.Add("Период", $"{SelectedMonth}.{SelectedYear}");

            ReportData = table;
        }

        private void ExportToExcel()
        {
            try
            {
                // TODO: Реализовать экспорт в Excel
                MessageBox.Show("Экспорт в Excel будет доступен в следующей версии",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}