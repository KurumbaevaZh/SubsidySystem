using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubsidySystem.Models
{
    [Table("subsidy_calculations")]
    public class SubsidyCalculation
    {
        [Key]
        [Column("calculation_id")]
        public int CalculationId { get; set; }

        [Required]
        [Column("citizen_id")]
        public int CitizenId { get; set; }

        [Required]
        [Column("calculation_date", TypeName = "date")]
        public DateTime CalculationDate { get; set; }

        [Required]
        [Column("year")]
        public int Year { get; set; }

        [Required]
        [Column("month")]
        public int Month { get; set; }

        [Required]
        [Column("total_family_income")]
        public decimal TotalFamilyIncome { get; set; }

        [Required]
        [Column("average_per_capita_income")]
        public decimal AveragePerCapitaIncome { get; set; }

        [Required]
        [Column("living_wage")]
        public decimal LivingWage { get; set; }

        [Column("correction_factor")]
        public decimal? CorrectionFactor { get; set; }

        [Required]
        [Column("housing_standard")]
        public decimal HousingStandard { get; set; }

        [Required]
        [Column("max_allowed_share")]
        public decimal MaxAllowedShare { get; set; }

        [Required]
        [Column("subsidy_amount")]
        public decimal SubsidyAmount { get; set; }

        [Required]
        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        // Навигационные свойства
        [ForeignKey("CitizenId")]
        public virtual Citizen Citizen { get; set; } = null!;
    }
}