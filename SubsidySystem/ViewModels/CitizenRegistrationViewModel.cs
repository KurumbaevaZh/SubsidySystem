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
    public class CitizenRegistrationViewModel : BaseViewModel
    {
        private readonly IRepository<Citizen> _citizenRepository;
        private readonly IRepository<Category> _categoryRepository;

        private Citizen _citizen = new();
        private ObservableCollection<Category> _categories = new();
        private Category? _selectedCategory;
        private bool _isSaving;
        private string _statusMessage = string.Empty;

        public CitizenRegistrationViewModel(
            IRepository<Citizen> citizenRepository,
            IRepository<Category> categoryRepository)
        {
            _citizenRepository = citizenRepository;
            _categoryRepository = categoryRepository;

            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave);
            CancelCommand = new RelayCommand(_ => Cancel());
            LoadCategoriesCommand = new RelayCommand(async _ => await LoadCategoriesAsync());

            Task.Run(async () => await LoadCategoriesAsync());
        }

        public Citizen Citizen
        {
            get => _citizen;
            set => SetField(ref _citizen, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetField(ref _categories, value);
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetField(ref _selectedCategory, value))
                {
                    if (value != null)
                        Citizen.CategoryId = value.CategoryId;
                }
            }
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

        public bool CanSave =>
            !string.IsNullOrWhiteSpace(Citizen.LastName) &&
            !string.IsNullOrWhiteSpace(Citizen.FirstName) &&
            Citizen.BirthDate != default &&
            !string.IsNullOrWhiteSpace(Citizen.RegistrationAddress);

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand LoadCategoriesCommand { get; }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = "Загрузка категорий...";
                });

                var categories = await _categoryRepository.GetAllAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Categories.Clear();
                    foreach (var cat in categories)
                        Categories.Add(cat);
                    StatusMessage = $"Загружено {Categories.Count} категорий";
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка загрузки категорий: {ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async Task SaveAsync()
        {
            IsSaving = true;
            StatusMessage = "Сохранение...";

            try
            {
                Citizen.CreatedAt = DateTime.UtcNow;

                await _citizenRepository.AddAsync(Citizen);
                await _citizenRepository.SaveChangesAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Заявитель {Citizen.LastName} {Citizen.FirstName} успешно зарегистрирован!";
                    MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                });

                Citizen = new Citizen();
                SelectedCategory = null;
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка: {ex.InnerException?.Message ?? ex.Message}";
                    MessageBox.Show(StatusMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void Cancel()
        {
            Citizen = new Citizen();
            SelectedCategory = null;
            StatusMessage = string.Empty;
        }
    }
}