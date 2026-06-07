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
    public class DictionariesViewModel : BaseViewModel
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<IncomeType> _incomeTypeRepository;
        private readonly IRepository<UtilityServiceType> _utilityServiceTypeRepository;

        // Категории
        private ObservableCollection<Category> _categories = new();
        private Category _category = new();
        private Category? _selectedCategory;

        // Виды доходов
        private ObservableCollection<IncomeType> _incomeTypes = new();
        private IncomeType _incomeType = new();
        private IncomeType? _selectedIncomeType;

        // Виды услуг ЖКУ
        private ObservableCollection<UtilityServiceType> _utilityServiceTypes = new();
        private UtilityServiceType _utilityServiceType = new();
        private UtilityServiceType? _selectedUtilityServiceType;

        private string _statusMessage = string.Empty;

        public DictionariesViewModel(
            IRepository<Category> categoryRepository,
            IRepository<IncomeType> incomeTypeRepository,
            IRepository<UtilityServiceType> utilityServiceTypeRepository)
        {
            _categoryRepository = categoryRepository;
            _incomeTypeRepository = incomeTypeRepository;
            _utilityServiceTypeRepository = utilityServiceTypeRepository;

            // Команды для категорий
            LoadCategoriesCommand = new RelayCommand(async _ => await LoadCategoriesAsync());
            AddCategoryCommand = new RelayCommand(async _ => await AddCategoryAsync(), _ => CanAddCategory);
            UpdateCategoryCommand = new RelayCommand(async _ => await UpdateCategoryAsync(), _ => SelectedCategory != null);
            DeleteCategoryCommand = new RelayCommand(async _ => await DeleteCategoryAsync(), _ => SelectedCategory != null);
            ClearCategoryCommand = new RelayCommand(_ => ClearCategoryForm());

            // Команды для видов доходов
            LoadIncomeTypesCommand = new RelayCommand(async _ => await LoadIncomeTypesAsync());
            AddIncomeTypeCommand = new RelayCommand(async _ => await AddIncomeTypeAsync(), _ => CanAddIncomeType);
            UpdateIncomeTypeCommand = new RelayCommand(async _ => await UpdateIncomeTypeAsync(), _ => SelectedIncomeType != null);
            DeleteIncomeTypeCommand = new RelayCommand(async _ => await DeleteIncomeTypeAsync(), _ => SelectedIncomeType != null);
            ClearIncomeTypeCommand = new RelayCommand(_ => ClearIncomeTypeForm());

            // Команды для видов услуг ЖКУ
            LoadUtilityServiceTypesCommand = new RelayCommand(async _ => await LoadUtilityServiceTypesAsync());
            AddUtilityServiceTypeCommand = new RelayCommand(async _ => await AddUtilityServiceTypeAsync(), _ => CanAddUtilityServiceType);
            UpdateUtilityServiceTypeCommand = new RelayCommand(async _ => await UpdateUtilityServiceTypeAsync(), _ => SelectedUtilityServiceType != null);
            DeleteUtilityServiceTypeCommand = new RelayCommand(async _ => await DeleteUtilityServiceTypeAsync(), _ => SelectedUtilityServiceType != null);
            ClearUtilityServiceTypeCommand = new RelayCommand(_ => ClearUtilityServiceTypeForm());

            // Загрузка данных
            Task.Run(async () =>
            {
                await LoadCategoriesAsync();
                await LoadIncomeTypesAsync();
                await LoadUtilityServiceTypesAsync();
            });
        }

        // ==================== Категории ====================
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetField(ref _categories, value);
        }

        public Category Category
        {
            get => _category;
            set => SetField(ref _category, value);
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetField(ref _selectedCategory, value) && value != null)
                {
                    Category = new Category
                    {
                        CategoryId = value.CategoryId,
                        CategoryName = value.CategoryName,
                        DiscountRate = value.DiscountRate,
                        EligibilityConditions = value.EligibilityConditions,
                        RegulatoryAct = value.RegulatoryAct
                    };
                }
            }
        }

        public bool CanAddCategory => !string.IsNullOrWhiteSpace(Category.CategoryName);

        // ==================== Виды доходов ====================
        public ObservableCollection<IncomeType> IncomeTypes
        {
            get => _incomeTypes;
            set => SetField(ref _incomeTypes, value);
        }

        public IncomeType IncomeType
        {
            get => _incomeType;
            set => SetField(ref _incomeType, value);
        }

        public IncomeType? SelectedIncomeType
        {
            get => _selectedIncomeType;
            set
            {
                if (SetField(ref _selectedIncomeType, value) && value != null)
                {
                    IncomeType = new IncomeType
                    {
                        IncomeTypeId = value.IncomeTypeId,
                        IncomeTypeName = value.IncomeTypeName,
                        IsConsidered = value.IsConsidered
                    };
                }
            }
        }

        public bool CanAddIncomeType => !string.IsNullOrWhiteSpace(IncomeType.IncomeTypeName);

        // ==================== Виды услуг ЖКУ ====================
        public ObservableCollection<UtilityServiceType> UtilityServiceTypes
        {
            get => _utilityServiceTypes;
            set => SetField(ref _utilityServiceTypes, value);
        }

        public UtilityServiceType UtilityServiceType
        {
            get => _utilityServiceType;
            set => SetField(ref _utilityServiceType, value);
        }

        public UtilityServiceType? SelectedUtilityServiceType
        {
            get => _selectedUtilityServiceType;
            set
            {
                if (SetField(ref _selectedUtilityServiceType, value) && value != null)
                {
                    UtilityServiceType = new UtilityServiceType
                    {
                        ServiceTypeId = value.ServiceTypeId,
                        ServiceTypeName = value.ServiceTypeName,
                        UnitOfMeasure = value.UnitOfMeasure
                    };
                }
            }
        }

        public bool CanAddUtilityServiceType => !string.IsNullOrWhiteSpace(UtilityServiceType.ServiceTypeName);

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        // Команды для категорий
        public ICommand LoadCategoriesCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand UpdateCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand ClearCategoryCommand { get; }

        // Команды для видов доходов
        public ICommand LoadIncomeTypesCommand { get; }
        public ICommand AddIncomeTypeCommand { get; }
        public ICommand UpdateIncomeTypeCommand { get; }
        public ICommand DeleteIncomeTypeCommand { get; }
        public ICommand ClearIncomeTypeCommand { get; }

        // Команды для видов услуг ЖКУ
        public ICommand LoadUtilityServiceTypesCommand { get; }
        public ICommand AddUtilityServiceTypeCommand { get; }
        public ICommand UpdateUtilityServiceTypeCommand { get; }
        public ICommand DeleteUtilityServiceTypeCommand { get; }
        public ICommand ClearUtilityServiceTypeCommand { get; }

        // ==================== Методы для категорий ====================
        private async Task LoadCategoriesAsync()
        {
            try
            {
                var items = await _categoryRepository.GetAllAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Categories.Clear();
                    foreach (var item in items.OrderBy(x => x.CategoryName))
                        Categories.Add(item);
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
        }

        private async Task AddCategoryAsync()
        {
            if (!CanAddCategory) return;

            try
            {
                await _categoryRepository.AddAsync(Category);
                await _categoryRepository.SaveChangesAsync();
                await LoadCategoriesAsync();
                ClearCategoryForm();
                StatusMessage = "Категория добавлена!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateCategoryAsync()
        {
            if (SelectedCategory == null) return;

            try
            {
                SelectedCategory.CategoryName = Category.CategoryName;
                SelectedCategory.DiscountRate = Category.DiscountRate;
                SelectedCategory.EligibilityConditions = Category.EligibilityConditions;
                SelectedCategory.RegulatoryAct = Category.RegulatoryAct;

                _categoryRepository.Update(SelectedCategory);
                await _categoryRepository.SaveChangesAsync();
                await LoadCategoriesAsync();
                ClearCategoryForm();
                StatusMessage = "Категория обновлена!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteCategoryAsync()
        {
            if (SelectedCategory == null) return;

            var result = MessageBox.Show($"Удалить категорию '{SelectedCategory.CategoryName}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                _categoryRepository.Delete(SelectedCategory);
                await _categoryRepository.SaveChangesAsync();
                await LoadCategoriesAsync();
                ClearCategoryForm();
                StatusMessage = "Категория удалена!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearCategoryForm()
        {
            Category = new Category();
            SelectedCategory = null;
        }

        // ==================== Методы для видов доходов ====================
        private async Task LoadIncomeTypesAsync()
        {
            try
            {
                var items = await _incomeTypeRepository.GetAllAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    IncomeTypes.Clear();
                    foreach (var item in items.OrderBy(x => x.IncomeTypeName))
                        IncomeTypes.Add(item);
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
        }

        private async Task AddIncomeTypeAsync()
        {
            if (!CanAddIncomeType) return;

            try
            {
                await _incomeTypeRepository.AddAsync(IncomeType);
                await _incomeTypeRepository.SaveChangesAsync();
                await LoadIncomeTypesAsync();
                ClearIncomeTypeForm();
                StatusMessage = "Вид дохода добавлен!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateIncomeTypeAsync()
        {
            if (SelectedIncomeType == null) return;

            try
            {
                SelectedIncomeType.IncomeTypeName = IncomeType.IncomeTypeName;
                SelectedIncomeType.IsConsidered = IncomeType.IsConsidered;

                _incomeTypeRepository.Update(SelectedIncomeType);
                await _incomeTypeRepository.SaveChangesAsync();
                await LoadIncomeTypesAsync();
                ClearIncomeTypeForm();
                StatusMessage = "Вид дохода обновлён!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteIncomeTypeAsync()
        {
            if (SelectedIncomeType == null) return;

            var result = MessageBox.Show($"Удалить вид дохода '{SelectedIncomeType.IncomeTypeName}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                _incomeTypeRepository.Delete(SelectedIncomeType);
                await _incomeTypeRepository.SaveChangesAsync();
                await LoadIncomeTypesAsync();
                ClearIncomeTypeForm();
                StatusMessage = "Вид дохода удалён!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearIncomeTypeForm()
        {
            IncomeType = new IncomeType();
            SelectedIncomeType = null;
        }

        // ==================== Методы для видов услуг ЖКУ ====================
        private async Task LoadUtilityServiceTypesAsync()
        {
            try
            {
                var items = await _utilityServiceTypeRepository.GetAllAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    UtilityServiceTypes.Clear();
                    foreach (var item in items.OrderBy(x => x.ServiceTypeName))
                        UtilityServiceTypes.Add(item);
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
        }

        private async Task AddUtilityServiceTypeAsync()
        {
            if (!CanAddUtilityServiceType) return;

            try
            {
                await _utilityServiceTypeRepository.AddAsync(UtilityServiceType);
                await _utilityServiceTypeRepository.SaveChangesAsync();
                await LoadUtilityServiceTypesAsync();
                ClearUtilityServiceTypeForm();
                StatusMessage = "Вид услуги добавлен!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateUtilityServiceTypeAsync()
        {
            if (SelectedUtilityServiceType == null) return;

            try
            {
                SelectedUtilityServiceType.ServiceTypeName = UtilityServiceType.ServiceTypeName;
                SelectedUtilityServiceType.UnitOfMeasure = UtilityServiceType.UnitOfMeasure;

                _utilityServiceTypeRepository.Update(SelectedUtilityServiceType);
                await _utilityServiceTypeRepository.SaveChangesAsync();
                await LoadUtilityServiceTypesAsync();
                ClearUtilityServiceTypeForm();
                StatusMessage = "Вид услуги обновлён!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteUtilityServiceTypeAsync()
        {
            if (SelectedUtilityServiceType == null) return;

            var result = MessageBox.Show($"Удалить вид услуги '{SelectedUtilityServiceType.ServiceTypeName}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                _utilityServiceTypeRepository.Delete(SelectedUtilityServiceType);
                await _utilityServiceTypeRepository.SaveChangesAsync();
                await LoadUtilityServiceTypesAsync();
                ClearUtilityServiceTypeForm();
                StatusMessage = "Вид услуги удалён!";
                MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
                MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearUtilityServiceTypeForm()
        {
            UtilityServiceType = new UtilityServiceType();
            SelectedUtilityServiceType = null;
        }
    }
}