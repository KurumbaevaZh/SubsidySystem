using SubsidySystem.Models;
using SubsidySystem.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SubsidySystem.Services
{
    public class SubsidyCalculationService : ISubsidyCalculationService
    {
        private readonly IRepository<Citizen> _citizenRepository;
        private readonly IRepository<FamilyMember> _familyMemberRepository;
        private readonly IRepository<Income> _incomeRepository;
        private readonly IRepository<Standard> _standardRepository;
        private readonly IRepository<SubsidyCalculation> _subsidyRepository;

        private const string LIVING_WAGE_WORKING = "Прожиточный минимум - трудоспособный";
        private const string LIVING_WAGE_PENSIONER = "Прожиточный минимум - пенсионер";
        private const string LIVING_WAGE_CHILD = "Прожиточный минимум - ребенок";
        private const string HOUSING_STANDARD_PREFIX = "Стандарт ЖКУ - ";
        private const string MAX_ALLOWED_SHARE = "Максимальная доля расходов";
        private const decimal DEFAULT_MAX_ALLOWED_SHARE_VALUE = 22;

        public SubsidyCalculationService(
            IRepository<Citizen> citizenRepository,
            IRepository<FamilyMember> familyMemberRepository,
            IRepository<Income> incomeRepository,
            IRepository<Standard> standardRepository,
            IRepository<SubsidyCalculation> subsidyRepository)
        {
            _citizenRepository = citizenRepository;
            _familyMemberRepository = familyMemberRepository;
            _incomeRepository = incomeRepository;
            _standardRepository = standardRepository;
            _subsidyRepository = subsidyRepository;
        }

        public async Task<SubsidyCalculationResult> CalculateSubsidyAsync(
            int citizenId,
            int year,
            int month,
            decimal totalFamilyIncome,
            int familySize,
            int workingAgeCount,
            int pensionerCount,
            int childCount)
        {
            var calculationDate = new DateTime(year, month, 1);
            var calculationDateOnly = DateOnly.FromDateTime(calculationDate);

            var livingWage = await CalculateFamilyLivingWageAsync(calculationDateOnly, workingAgeCount, pensionerCount, childCount);
            if (livingWage == 0) livingWage = 15000;

            var averagePerCapitaIncome = totalFamilyIncome / familySize;

            var housingStandardCost = await GetHousingStandardCostAsync(calculationDateOnly, familySize);
            if (housingStandardCost == 0) housingStandardCost = 5000;

            var maxAllowedShare = await GetMaxAllowedShareAsync(calculationDateOnly);

            bool isBelowLivingWage = averagePerCapitaIncome < livingWage;
            decimal subsidyAmount;
            decimal correctionFactor = 1;
            string calculationFormula;

            if (isBelowLivingWage)
            {
                correctionFactor = averagePerCapitaIncome / livingWage;
                subsidyAmount = housingStandardCost - (maxAllowedShare / 100 * totalFamilyIncome) * correctionFactor;
                calculationFormula = $"C = {housingStandardCost:F2} - ({maxAllowedShare} / 100) × {totalFamilyIncome:F2} × {correctionFactor:F3}";
            }
            else
            {
                subsidyAmount = housingStandardCost - (maxAllowedShare / 100 * totalFamilyIncome);
                calculationFormula = $"C = {housingStandardCost:F2} - ({maxAllowedShare} / 100) × {totalFamilyIncome:F2}";
            }

            if (subsidyAmount < 0) subsidyAmount = 0;

            return new SubsidyCalculationResult
            {
                SubsidyAmount = Math.Round(subsidyAmount, 2),
                AveragePerCapitaIncome = Math.Round(averagePerCapitaIncome, 2),
                LivingWage = Math.Round(livingWage, 2),
                CorrectionFactor = Math.Round(correctionFactor, 4),
                HousingStandardCost = Math.Round(housingStandardCost, 2),
                MaxAllowedShare = maxAllowedShare,
                CalculationFormula = calculationFormula,
                IsBelowLivingWage = isBelowLivingWage
            };
        }

        public async Task<SubsidyCalculationResult> CalculateFullSubsidyAsync(int citizenId, int year, int month)
        {
            var calculationDate = new DateTime(year, month, 1);
            var calculationDateOnly = DateOnly.FromDateTime(calculationDate);

            var citizen = await _citizenRepository.GetByIdAsync(citizenId);
            if (citizen == null)
                throw new Exception("Заявитель не найден");

            var familyMembers = await _familyMemberRepository.FindAsync(f => f.CitizenId == citizenId);
            var familySize = familyMembers.Count() + 1;

            int workingAgeCount = 0;
            int pensionerCount = 0;
            int childCount = 0;

            var citizenBirthDate = citizen.BirthDate.ToDateTime(TimeOnly.MinValue);
            if (citizenBirthDate.AddYears(60) <= calculationDate)
                pensionerCount++;
            else if (citizenBirthDate.AddYears(18) > calculationDate)
                childCount++;
            else
                workingAgeCount++;

            foreach (var member in familyMembers)
            {
                var memberBirthDate = member.BirthDate.ToDateTime(TimeOnly.MinValue);
                if (memberBirthDate.AddYears(60) <= calculationDate)
                    pensionerCount++;
                else if (memberBirthDate.AddYears(18) > calculationDate)
                    childCount++;
                else
                    workingAgeCount++;
            }

            var incomes = new List<Income>();
            foreach (var member in familyMembers)
            {
                var memberIncomes = await _incomeRepository.FindAsync(i => i.MemberId == member.MemberId);
                incomes.AddRange(memberIncomes);
            }
            var totalFamilyIncome = incomes.Sum(i => i.Amount);

            var result = await CalculateSubsidyAsync(
                citizenId, year, month,
                totalFamilyIncome, familySize,
                workingAgeCount, pensionerCount, childCount);

            try
            {
                // Проверка существующего расчета
                var existingCalculations = await _subsidyRepository
                    .FindAsync(s => s.CitizenId == citizenId && s.Year == year && s.Month == month);

                if (existingCalculations.Any())
                {
                    var existing = existingCalculations.First();
                    existing.TotalFamilyIncome = totalFamilyIncome;
                    existing.AveragePerCapitaIncome = result.AveragePerCapitaIncome;
                    existing.LivingWage = result.LivingWage;
                    existing.CorrectionFactor = result.CorrectionFactor;
                    existing.HousingStandard = result.HousingStandardCost;
                    existing.MaxAllowedShare = result.MaxAllowedShare;
                    existing.SubsidyAmount = result.SubsidyAmount;
                    existing.Status = "назначено";
                    existing.CalculationDate = calculationDateOnly.ToDateTime(TimeOnly.MinValue);

                    _subsidyRepository.Update(existing);
                }
                else
                {
                    var calculation = new SubsidyCalculation
                    {
                        CitizenId = citizenId,
                        CalculationDate = calculationDateOnly.ToDateTime(TimeOnly.MinValue),
                        Year = year,
                        Month = month,
                        TotalFamilyIncome = totalFamilyIncome,
                        AveragePerCapitaIncome = result.AveragePerCapitaIncome,
                        LivingWage = result.LivingWage,
                        CorrectionFactor = result.CorrectionFactor,
                        HousingStandard = result.HousingStandardCost,
                        MaxAllowedShare = result.MaxAllowedShare,
                        SubsidyAmount = result.SubsidyAmount,
                        Status = "назначено",
                        Notes = result.CalculationFormula,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _subsidyRepository.AddAsync(calculation);
                }

                await _subsidyRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Ошибка при сохранении расчета: {ex.InnerException?.Message ?? ex.Message}",
                        "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                throw;
            }

            return result;
        }

        private async Task<decimal> CalculateFamilyLivingWageAsync(
            DateOnly calculationDate,
            int workingAgeCount,
            int pensionerCount,
            int childCount)
        {
            var workingAgeWage = await GetStandardValueAsync(LIVING_WAGE_WORKING, calculationDate);
            var pensionerWage = await GetStandardValueAsync(LIVING_WAGE_PENSIONER, calculationDate);
            var childWage = await GetStandardValueAsync(LIVING_WAGE_CHILD, calculationDate);

            int totalCount = workingAgeCount + pensionerCount + childCount;
            if (totalCount == 0) return 0;

            return (workingAgeWage * workingAgeCount + pensionerWage * pensionerCount + childWage * childCount) / totalCount;
        }

        private async Task<decimal> GetStandardValueAsync(string standardType, DateOnly calculationDate)
        {
            var calculationDateTime = calculationDate.ToDateTime(TimeOnly.MinValue);

            var standard = await _standardRepository
                .FindAsync(s => s.StandardType == standardType &&
                                 s.ValidFrom <= calculationDateTime &&
                                 (s.ValidTo == null || s.ValidTo >= calculationDateTime))
                .ContinueWith(t => t.Result.FirstOrDefault());

            return standard?.StandardValue ?? 0;
        }

        private async Task<decimal> GetHousingStandardCostAsync(DateOnly calculationDate, int familySize)
        {
            var calculationDateTime = calculationDate.ToDateTime(TimeOnly.MinValue);
            string standardType = $"{HOUSING_STANDARD_PREFIX}{familySize} чел";

            var standard = await _standardRepository
                .FindAsync(s => s.StandardType == standardType &&
                                 s.ValidFrom <= calculationDateTime &&
                                 (s.ValidTo == null || s.ValidTo >= calculationDateTime))
                .ContinueWith(t => t.Result.FirstOrDefault());

            if (standard != null)
                return standard.StandardValue;

            var allStandards = await _standardRepository
                .FindAsync(s => s.StandardType.StartsWith(HOUSING_STANDARD_PREFIX) &&
                                 s.ValidFrom <= calculationDateTime &&
                                 (s.ValidTo == null || s.ValidTo >= calculationDateTime))
                .ContinueWith(t => t.Result.ToList());

            var fallbackStandard = allStandards
                .Select(s => new
                {
                    Standard = s,
                    Size = int.Parse(s.StandardType.Replace(HOUSING_STANDARD_PREFIX, "").Replace(" чел", ""))
                })
                .OrderBy(x => Math.Abs(x.Size - familySize))
                .FirstOrDefault();

            return fallbackStandard?.Standard.StandardValue ?? 0;
        }

        private async Task<decimal> GetMaxAllowedShareAsync(DateOnly calculationDate)
        {
            var calculationDateTime = calculationDate.ToDateTime(TimeOnly.MinValue);

            var standard = await _standardRepository
                .FindAsync(s => s.StandardType == MAX_ALLOWED_SHARE &&
                                 s.ValidFrom <= calculationDateTime &&
                                 (s.ValidTo == null || s.ValidTo >= calculationDateTime))
                .ContinueWith(t => t.Result.FirstOrDefault());

            if (standard != null)
                return standard.StandardValue;

            return DEFAULT_MAX_ALLOWED_SHARE_VALUE;
        }

    }
}