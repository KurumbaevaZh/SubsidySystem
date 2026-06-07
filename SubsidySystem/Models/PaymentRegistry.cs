using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubsidySystem.Models
{
    [Table("payment_registries")]
    public class PaymentRegistry
    {
        [Key]
        [Column("registry_id")]
        public int RegistryId { get; set; }

        [Required]
        [Column("registry_number")]
        [MaxLength(20)]
        public string RegistryNumber { get; set; } = string.Empty;

        [Required]
        [Column("registry_date", TypeName = "date")]
        public DateTime RegistryDate { get; set; }

        [Required]
        [Column("period_year")]
        public int PeriodYear { get; set; }

        [Required]
        [Column("period_month")]
        public int PeriodMonth { get; set; }

        [Required]
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column("recipient_count")]
        public int RecipientCount { get; set; }

        [Column("status")]
        [MaxLength(20)]
        public string? Status { get; set; } = "формируется";

        // НОВЫЕ ПОЛЯ - добавьте их
        [Column("approved_by")]
        [MaxLength(100)]
        public string? ApprovedBy { get; set; }

        [Column("approved_date", TypeName = "date")]
        public DateTime? ApprovedDate { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        // Навигационное свойство
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}