using System.Threading.Tasks;

namespace SubsidySystem.Services
{
    public interface ISubsidyCalculationService
    {
        /// <summary>
        /// Расчет субсидии
        /// </summary>
        /// <param name="citizenId">Идентификатор заявителя</param>
        /// <param name="year">Год расчета</param>
        /// <param name="month">Месяц расчета</param>
        /// <param name="totalFamilyIncome">Совокупный доход семьи за расчетный период</param>
        /// <param name="familySize">Количество членов семьи</param>
        /// <param name="workingAgeCount">Количество трудоспособных</param>
        /// <param name="pensionerCount">Количество пенсионеров</param>
        /// <param name="childCount">Количество детей</param>
        /// <returns>Результат расчета субсидии</returns>
        Task<SubsidyCalculationResult> CalculateSubsidyAsync(
            int citizenId,
            int year,
            int month,
            decimal totalFamilyIncome,
            int familySize,
            int workingAgeCount,
            int pensionerCount,
            int childCount);

        /// <summary>
        /// Полный расчет субсидии с получением данных из базы
        /// </summary>
        Task<SubsidyCalculationResult> CalculateFullSubsidyAsync(int citizenId, int year, int month);
    }

    /// <summary>
    /// Результат расчета субсидии
    /// </summary>
    public class SubsidyCalculationResult
    {
        public decimal SubsidyAmount { get; set; }
        public decimal AveragePerCapitaIncome { get; set; }
        public decimal LivingWage { get; set; }
        public decimal CorrectionFactor { get; set; }
        public decimal HousingStandardCost { get; set; }
        public decimal MaxAllowedShare { get; set; }
        public string CalculationFormula { get; set; } = string.Empty;
        public bool IsBelowLivingWage { get; set; }
    }
}