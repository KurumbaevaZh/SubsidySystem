using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubsidySystem.Models
{
    [Table("compensation_calculations")]
    public class CompensationCalculation
    {
        [Key]
        [Column("compensation_id")]
        public int CompensationId { get; set; }

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
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [Column("discount_rate")]
        public decimal DiscountRate { get; set; }

        [Required]
        [Column("actual_charges")]
        public decimal ActualCharges { get; set; }

        [Column("social_norm")]
        public decimal? SocialNorm { get; set; }

        [Column("consumption_rate")]
        public decimal? ConsumptionRate { get; set; }

        [Required]
        [Column("compensation_amount")]
        public decimal CompensationAmount { get; set; }

        [Required]
        [Column("status")]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CitizenId")]
        public virtual Citizen? Citizen { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}