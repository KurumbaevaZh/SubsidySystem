using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubsidySystem.Models
{
    [Table("payments")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Required]
        [Column("registry_id")]
        public int RegistryId { get; set; }

        [Required]
        [Column("citizen_id")]
        public int CitizenId { get; set; }

        [Required]
        [Column("payment_type")]
        [MaxLength(30)]
        public string PaymentType { get; set; } = string.Empty;

        [Required]
        [Column("source_calculation_id")]
        public int SourceCalculationId { get; set; }

        [Required]
        [Column("payment_amount")]
        public decimal PaymentAmount { get; set; }

        [Column("payment_method")]
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [Column("payment_details")]
        [MaxLength(100)]
        public string? PaymentDetails { get; set; }

        [Column("payment_date", TypeName = "date")]
        public DateTime? PaymentDate { get; set; }  // nullable

        [Column("status")]
        [MaxLength(20)]
        public string? Status { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }  // nullable

        [ForeignKey("RegistryId")]
        public virtual PaymentRegistry? Registry { get; set; }

        [ForeignKey("CitizenId")]
        public virtual Citizen? Citizen { get; set; }
    }
}