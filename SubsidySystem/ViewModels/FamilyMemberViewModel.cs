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
    public class FamilyMemberViewModel : BaseViewModel
    {
        private readonly IRepository<Citizen> _citizenRepository;
        private readonly IRepository<FamilyMember> _familyMemberRepository;

        private ObservableCollection<Citizen> _citizens = new();
        private Citizen? _selectedCitizen;
        private ObservableCollection<FamilyMember> _familyMembers = new();
        private FamilyMember _familyMember = new();
        private FamilyMember? _selectedFamilyMember;
        private bool _isSaving;
        private string _statusMessage = string.Empty;

        private ObservableCollection<string> _relationships = new()
        {
            "Супруг", "Супруга", "Сын", "Дочь", "Отец", "Мать",
            "Брат", "Сестра", "Дед", "Бабушка", "Внук", "Внучка",
            "Опекун", "Подопечный", "Другой"
        };

        public FamilyMemberViewModel(
            IRepository<Citizen> citizenRepository,
            IRepository<FamilyMember> familyMemberRepository)
        {
            _citizenRepository = citizenRepository;
            _familyMemberRepository = familyMemberRepository;

            LoadCitizensCommand = new RelayCommand(async _ => await LoadCitizensAsync());
            LoadFamilyMembersCommand = new RelayCommand(async _ => await LoadFamilyMembersAsync(), _ => SelectedCitizen != null);
            AddCommand = new RelayCommand(async _ => await AddAsync(), _ => CanAdd);
            UpdateCommand = new RelayCommand(async _ => await UpdateAsync(), _ => SelectedFamilyMember != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedFamilyMember != null);
            ClearCommand = new RelayCommand(_ => ClearForm());

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
            set
            {
                if (SetField(ref _selectedCitizen, value))
                {
                    ClearForm();
                    if (value != null)
                    {
                        LoadFamilyMembersCommand.Execute(null);
                    }
                    else
                    {
                        FamilyMembers.Clear();
                    }
                }
            }
        }

        public ObservableCollection<FamilyMember> FamilyMembers
        {
            get => _familyMembers;
            set => SetField(ref _familyMembers, value);
        }

        public FamilyMember FamilyMember
        {
            get => _familyMember;
            set => SetField(ref _familyMember, value);
        }

        public FamilyMember? SelectedFamilyMember
        {
            get => _selectedFamilyMember;
            set
            {
                if (SetField(ref _selectedFamilyMember, value) && value != null)
                {
                    FamilyMember = new FamilyMember
                    {
                        MemberId = value.MemberId,
                        CitizenId = value.CitizenId,
                        LastName = value.LastName,
                        FirstName = value.FirstName,
                        MiddleName = value.MiddleName,
                        BirthDate = value.BirthDate,
                        Relationship = value.Relationship,
                        IsStudent = value.IsStudent,
                        Snils = value.Snils,
                        Notes = value.Notes
                    };
                }
            }
        }

        public ObservableCollection<string> Relationships => _relationships;

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

        public bool CanAdd =>
            SelectedCitizen != null &&
            !string.IsNullOrWhiteSpace(FamilyMember.LastName) &&
            !string.IsNullOrWhiteSpace(FamilyMember.FirstName) &&
            FamilyMember.BirthDate != default &&
            !string.IsNullOrWhiteSpace(FamilyMember.Relationship);

        public ICommand LoadCitizensCommand { get; }
        public ICommand LoadFamilyMembersCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }

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

        private async Task LoadFamilyMembersAsync()
        {
            if (SelectedCitizen == null) return;

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
                    foreach (var m in members.OrderBy(m => m.Relationship))
                        FamilyMembers.Add(m);
                    StatusMessage = $"Загружено {FamilyMembers.Count} членов семьи";
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

        private async Task AddAsync()
        {
            if (!CanAdd) return;

            IsSaving = true;
            StatusMessage = "Добавление члена семьи...";

            try
            {
                var newMember = new FamilyMember
                {
                    CitizenId = SelectedCitizen!.CitizenId,
                    LastName = FamilyMember.LastName,
                    FirstName = FamilyMember.FirstName,
                    MiddleName = FamilyMember.MiddleName,
                    BirthDate = FamilyMember.BirthDate,
                    Relationship = FamilyMember.Relationship,
                    IsStudent = FamilyMember.IsStudent,
                    Snils = FamilyMember.Snils,
                    Notes = FamilyMember.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _familyMemberRepository.AddAsync(newMember);
                await _familyMemberRepository.SaveChangesAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = "Член семьи успешно добавлен!";
                    MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                });

                ClearForm();
                await LoadFamilyMembersAsync();
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

        private async Task UpdateAsync()
        {
            if (SelectedFamilyMember == null) return;

            IsSaving = true;
            StatusMessage = "Обновление данных...";

            try
            {
                SelectedFamilyMember.LastName = FamilyMember.LastName;
                SelectedFamilyMember.FirstName = FamilyMember.FirstName;
                SelectedFamilyMember.MiddleName = FamilyMember.MiddleName;
                SelectedFamilyMember.BirthDate = FamilyMember.BirthDate;
                SelectedFamilyMember.Relationship = FamilyMember.Relationship;
                SelectedFamilyMember.IsStudent = FamilyMember.IsStudent;
                SelectedFamilyMember.Snils = FamilyMember.Snils;
                SelectedFamilyMember.Notes = FamilyMember.Notes;
                SelectedFamilyMember.UpdatedAt = DateTime.UtcNow;

                _familyMemberRepository.Update(SelectedFamilyMember);
                await _familyMemberRepository.SaveChangesAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = "Данные успешно обновлены!";
                    MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                });

                ClearForm();
                await LoadFamilyMembersAsync();
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

        private async Task DeleteAsync()
        {
            if (SelectedFamilyMember == null) return;

            var result = MessageBox.Show($"Удалить {SelectedFamilyMember.LastName} {SelectedFamilyMember.FirstName} из списка членов семьи?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            IsSaving = true;
            StatusMessage = "Удаление...";

            try
            {
                _familyMemberRepository.Delete(SelectedFamilyMember);
                await _familyMemberRepository.SaveChangesAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = "Член семьи удален!";
                    MessageBox.Show(StatusMessage, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                });

                ClearForm();
                await LoadFamilyMembersAsync();
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

        private void ClearForm()
        {
            FamilyMember = new FamilyMember();
            SelectedFamilyMember = null;
            StatusMessage = "Форма очищена";
        }
    }
}