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
    public class IncomeViewModel : BaseViewModel
    {
        private readonly IRepository<Citizen> _citizenRepository;
        private readonly IRepository<FamilyMember> _familyMemberRepository;
        private readonly IRepository<IncomeType> _incomeTypeRepository;
        private readonly IRepository<Income> _incomeRepository;

        private ObservableCollection<Citizen> _citizens = new();
        private Citizen? _selectedCitizen;
        private ObservableCollection<FamilyMember> _familyMembers = new();
        private FamilyMember? _selectedFamilyMember;
        private ObservableCollection<IncomeType> _incomeTypes = new();
        private IncomeType? _selectedIncomeType;
        private Income _income = new();
        private ObservableCollection<Income> _incomes = new();
        private bool _isSaving;
        private string _statusMessage = string.Empty;
        private Income? _selectedIncome;

        public IncomeViewModel(
            IRepository<Citizen> citizenRepository,
            IRepository<FamilyMember> familyMemberRepository,
            IRepository<IncomeType> incomeTypeRepository,
            IRepository<Income> incomeRepository)
        {
            _citizenRepository = citizenRepository;
            _familyMemberRepository = familyMemberRepository;
            _incomeTypeRepository = incomeTypeRepository;
            _incomeRepository = incomeRepository;

            LoadCitizensCommand = new RelayCommand(async _ => await LoadCitizensAsync());
            LoadFamilyMembersCommand = new RelayCommand(async _ => await LoadFamilyMembersAsync(), _ => SelectedCitizen != null);
            LoadIncomeTypesCommand = new RelayCommand(async _ => await LoadIncomeTypesAsync());
            LoadIncomesCommand = new RelayCommand(async _ => await LoadIncomesAsync(), _ => SelectedFamilyMember != null);
            AddIncomeCommand = new RelayCommand(async _ => await AddIncomeAsync(), _ => CanAddIncome);
            DeleteIncomeCommand = new RelayCommand(async param => await DeleteIncomeAsync(param), _ => SelectedIncome != null);
            SaveAllCommand = new RelayCommand(async _ => await SaveAllAsync(), _ => Incomes.Any());

            Task.Run(async () => {
                await LoadCitizensAsync();
                await LoadIncomeTypesAsync();
            });
        }

        public ObservableCollection<Citizen> Citizens
        {
            get => _citizens;
            set => SetField(ref _citizens, value);
        }

        public Citizen? SelectedCitizen
        {
            get => _selectedCitizen;
            set
            {
                if (SetField(ref _selectedCitizen, value))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FamilyMembers.Clear();
                        Incomes.Clear();
                    });
                    SelectedFamilyMember = null;

                    if (value != null)
                    {
                        LoadFamilyMembersCommand.Execute(null);
                    }
                }
            }
        }

        public ObservableCollection<FamilyMember> FamilyMembers
        {
            get => _familyMembers;
            set => SetField(ref _familyMembers, value);
        }

        public FamilyMember? SelectedFamilyMember
        {
            get => _selectedFamilyMember;
            set
            {
                if (SetField(ref _selectedFamilyMember, value) && value != null)
                {
                    LoadIncomesCommand.Execute(null);
                }
            }
        }

        public ObservableCollection<IncomeType> IncomeTypes
        {
            get => _incomeTypes;
            set => SetField(ref _incomeTypes, value);
        }

        public IncomeType? SelectedIncomeType
        {
            get => _selectedIncomeType;
            set
            {
                if (SetField(ref _selectedIncomeType, value) && value != null)
                {
                    Income.IncomeTypeId = value.IncomeTypeId;
                }
            }
        }

        public Income Income
        {
            get => _income;
            set => SetField(ref _income, value);
        }

        public ObservableCollection<Income> Incomes
        {
            get => _incomes;
            set => SetField(ref _incomes, value);
        }

        public Income? SelectedIncome
        {
            get => _selectedIncome;
            set => SetField(ref _selectedIncome, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetField(ref _isSaving, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public bool CanAddIncome =>
            SelectedFamilyMember != null &&
            SelectedIncomeType != null &&
            Income.Amount > 0 &&
            Income.PeriodStart != default &&
            Income.PeriodEnd != default &&
            Income.PeriodEnd >= Income.PeriodStart;

        public ICommand LoadCitizensCommand { get; }
        public ICommand LoadFamilyMembersCommand { get; }
        public ICommand LoadIncomeTypesCommand { get; }
        public ICommand LoadIncomesCommand { get; }
        public ICommand AddIncomeCommand { get; }
        public ICommand DeleteIncomeCommand { get; }
        public ICommand SaveAllCommand { get; }

        private async Task LoadCitizensAsync()
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = "Загрузка списка граждан...";
                });

                var citizens = await _citizenRepository.GetAllAsync();
                var previousCitizenId = SelectedCitizen?.CitizenId;

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Citizens.Clear();
                    foreach (var c in citizens.OrderBy(c => c.LastName))
                        Citizens.Add(c);

                    if (previousCitizenId.HasValue)
                    {
                        SelectedCitizen = Citizens.FirstOrDefault(c => c.CitizenId == previousCitizenId.Value);
                    }

                    StatusMessage = $"Загружено {Citizens.Count} граждан";
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка загрузки граждан: {ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async Task LoadFamilyMembersAsync()
        {
            if (SelectedCitizen == null)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => FamilyMembers.Clear());
                return;
            }

            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = "Загрузка членов семьи...";
                });

                var members = await _familyMemberRepository
                    .FindAsync(f => f.CitizenId == SelectedCitizen.CitizenId);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    FamilyMembers.Clear();
                    foreach (var m in members)
                        FamilyMembers.Add(m);

                    StatusMessage = $"Загружено {FamilyMembers.Count} членов семьи";
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка загрузки членов семьи: {ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async Task LoadIncomeTypesAsync()
        {
            try
            {
                var types = await _incomeTypeRepository.GetAllAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    IncomeTypes.Clear();
                    foreach (var t in types.OrderBy(t => t.IncomeTypeName))
                        IncomeTypes.Add(t);
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка загрузки видов доходов: {ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async Task LoadIncomesAsync()
        {
            if (SelectedFamilyMember == null)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => Incomes.Clear());
                return;
            }

            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = "Загрузка доходов...";
                });

                var incomes = await _incomeRepository
                    .FindAsync(i => i.MemberId == SelectedFamilyMember.MemberId);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Incomes.Clear();
                    foreach (var i in incomes)
                        Incomes.Add(i);

                    StatusMessage = $"Загружено {Incomes.Count} записей о доходах";
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка загрузки доходов: {ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async Task AddIncomeAsync()
        {
            if (!CanAddIncome) return;

            try
            {
                var newIncome = new Income
                {
                    MemberId = SelectedFamilyMember!.MemberId,
                    IncomeTypeId = SelectedIncomeType!.IncomeTypeId,
                    Amount = Income.Amount,
                    PeriodStart = Income.PeriodStart,
                    PeriodEnd = Income.PeriodEnd,
                    DocumentName = Income.DocumentName,
                    CreatedAt = DateTime.UtcNow
                };

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Incomes.Add(newIncome);
                    Income = new Income();
                    SelectedIncomeType = null;
                    StatusMessage = "Доход добавлен в список. Не забудьте сохранить!";
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка добавления дохода: {ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async Task DeleteIncomeAsync(object param)
        {
            if (param is Income income)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Incomes.Remove(income);
                    StatusMessage = "Доход удален из списка. Не забудьте сохранить!";
                });
            }
        }

        private async Task SaveAllAsync()
        {
            IsSaving = true;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                StatusMessage = "Сохранение...";
            });

            try
            {
                foreach (var income in Incomes)
                {
                    if (income.IncomeId == 0)
                        await _incomeRepository.AddAsync(income);
                    else
                        _incomeRepository.Update(income);
                }
                await _incomeRepository.SaveChangesAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Сохранено {Incomes.Count} записей о доходах!";
                    MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                });

                await LoadIncomesAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                IsSaving = false;
            }
        }
    }
}