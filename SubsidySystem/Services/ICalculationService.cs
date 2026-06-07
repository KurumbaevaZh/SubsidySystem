using Microsoft.EntityFrameworkCore;
using SubsidySystem.Models;
using SubsidySystem.Repositories;

namespace SubsidySystem.Services
{
    public interface ICalculationService
    {
        Task<SubsidyCalculation> CalculateSubsidyAsync(int citizenId, int year, int month);
        Task<CompensationCalculation> CalculateCompensationAsync(int citizenId, int year, int month);
    }

    public class CalculationService : ICalculationService
    {
        private readonly IRepository<Citizen> _citizenRepository;
        private readonly IRepository<FamilyMember> _familyMemberRepository;
        private readonly IRepository<Income> _incomeRepository;
        private readonly IRepository<Standard> _standardRepository;
        private readonly IRepository<UtilityCharge> _utilityChargeRepository;
        private readonly IRepository<SubsidyCalculation> _subsidyRepository;
        private readonly IRepository<CompensationCalculation> _compensationRepository;

        public CalculationService(
            IRepository<Citizen> citizenRepository,
            IRepository<FamilyMember> familyMemberRepository,
            IRepository<Income> incomeRepository,
            IRepository<Standard> standardRepository,
            IRepository<UtilityCharge> utilityChargeRepository,
            IRepository<SubsidyCalculation> subsidyRepository,
            IRepository<CompensationCalculation> compensationRepository)
        {
            _citizenRepository = citizenRepository;
            _familyMemberRepository = familyMemberRepository;
            _incomeRepository = incomeRepository;
            _standardRepository = standardRepository;
            _utilityChargeRepository = utilityChargeRepository;
            _subsidyRepository = subsidyRepository;
            _compensationRepository = compensationRepository;
        }

        public async Task<SubsidyCalculation> CalculateSubsidyAsync(int citizenId, int year, int month)
        {
            // 1. Получение данных о заявителе и членах семьи
            var citizen = await _citizenRepository.GetByIdAsync(citizenId);
            if (citizen == null)
                throw new Exception("Заявитель не найден");

            var familyMembers = await _familyMemberRepository
                .FindAsync(f => f.CitizenId == citizenId);
            var familyCount = familyMembers.Count() + 1; // + заявитель

            // 2. Расчет совокупного дохода семьи
            var incomes = new List<Income>();
            foreach (var member in familyMembers)
            {
                var memberIncomes = await _incomeRepository
                    .FindAsync(i => i.MemberId == member.MemberId);
                incomes.AddRange(memberIncomes);
            }
            var totalFamilyIncome = incomes.Sum(i => i.Amount);
            var averagePerCapitaIncome = totalFamilyIncome / familyCount;

            // 3. Получение нормативов
            var livingWage = await _standardRepository
                .FindAsync(s => s.StandardType == "Прожиточный минимум" &&
                                 s.ValidFrom <= DateTime.Now &&
                                 (s.ValidTo == null || s.ValidTo >= DateTime.Now))
                .ContinueWith(t => t.Result.FirstOrDefault()?.StandardValue ?? 0);

            var housingStandard = await _standardRepository
                .FindAsync(s => s.StandardType == "Стандарт стоимости ЖКУ" &&
                                 s.ValidFrom <= DateTime.Now &&
                                 (s.ValidTo == null || s.ValidTo >= DateTime.Now))
                .ContinueWith(t => t.Result.FirstOrDefault()?.StandardValue ?? 0);

            var maxAllowedShare = await _standardRepository
                .FindAsync(s => s.StandardType == "Максимальная доля расходов" &&
                                 s.ValidFrom <= DateTime.Now &&
                                 (s.ValidTo == null || s.ValidTo >= DateTime.Now))
                .ContinueWith(t => t.Result.FirstOrDefault()?.StandardValue ?? 22);

            // 4. Расчет поправочного коэффициента (при необходимости)
            decimal correctionFactor = 1;
            if (averagePerCapitaIncome < livingWage && livingWage > 0)
            {
                correctionFactor = averagePerCapitaIncome / livingWage;
            }

            // 5. Расчет субсидии
            var standardCost = housingStandard * familyCount;
            var subsidyAmount = standardCost - (maxAllowedShare / 100 * totalFamilyIncome) * correctionFactor;
            if (subsidyAmount < 0) subsidyAmount = 0;

            // 6. Сохранение результата
            var calculation = new SubsidyCalculation
            {
                CitizenId = citizenId,
                CalculationDate = DateTime.Now,
                Year = year,
                Month = month,
                TotalFamilyIncome = totalFamilyIncome,
                AveragePerCapitaIncome = averagePerCapitaIncome,
                LivingWage = livingWage,
                CorrectionFactor = correctionFactor,
                HousingStandard = housingStandard,
                MaxAllowedShare = maxAllowedShare,
                SubsidyAmount = subsidyAmount,
                Status = "назначено"
            };

            await _subsidyRepository.AddAsync(calculation);
            await _subsidyRepository.SaveChangesAsync();

            return calculation;
        }

        public async Task<CompensationCalculation> CalculateCompensationAsync(int citizenId, int year, int month)
        {
            // 1. Получение данных о заявителе и его категории
            var citizen = await _citizenRepository.GetByIdAsync(citizenId);
            if (citizen == null)
                throw new Exception("Заявитель не найден");

            // 2. Получение фактических начислений ЖКУ
            var utilityCharges = await _utilityChargeRepository
                .FindAsync(u => u.CitizenId == citizenId && u.Year == year && u.Month == month);
            var actualCharges = utilityCharges.Sum(u => u.ChargeAmount);

            // 3. Получение скидки по категории льготника
            var discountRate = citizen.Category?.DiscountRate ?? 0;

            // 4. Расчет компенсации
            var compensationAmount = actualCharges * (discountRate / 100);

            // 5. Сохранение результата
            var calculation = new CompensationCalculation
            {
                CitizenId = citizenId,
                CalculationDate = DateTime.Now,
                Year = year,
                Month = month,
                CategoryId = citizen.CategoryId ?? 0,
                DiscountRate = discountRate,
                ActualCharges = actualCharges,
                CompensationAmount = compensationAmount,
                Status = "назначено"
            };

            await _compensationRepository.AddAsync(calculation);
            await _compensationRepository.SaveChangesAsync();

            return calculation;
        }
    }
}