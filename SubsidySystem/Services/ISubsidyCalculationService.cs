using System.Threading.Tasks;

namespace SubsidySystem.Services
{
    public interface ISubsidyCalculationService
    {
        Task<SubsidyCalculationResult> CalculateSubsidyAsync(
            int citizenId,
            int year,
            int month,
            decimal totalFamilyIncome,
            int familySize,
            int workingAgeCount,
            int pensionerCount,
            int childCount);

        Task<SubsidyCalculationResult> CalculateFullSubsidyAsync(int citizenId, int year, int month);
    }

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