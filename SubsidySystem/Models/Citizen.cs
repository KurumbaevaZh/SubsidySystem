using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubsidySystem.Models
{
    [Table("citizens")]
    public class Citizen
    {
        [Key]
        [Column("citizen_id")]
        public int CitizenId { get; set; }

        [Required]
        [Column("last_name")]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Column("first_name")]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Column("middle_name")]
        [MaxLength(50)]
        public string? MiddleName { get; set; }

        [Required]
        [Column("birth_date", TypeName = "date")]
        public DateOnly BirthDate { get; set; }

        [Column("snils")]
        [MaxLength(14)]
        public string? Snils { get; set; }

        [Column("passport_series")]
        [MaxLength(4)]
        public string? PassportSeries { get; set; }

        [Column("passport_number")]
        [MaxLength(6)]
        public string? PassportNumber { get; set; }

        [Column("passport_issuer")]
        [MaxLength(200)]
        public string? PassportIssuer { get; set; }

        [Column("passport_issue_date", TypeName = "date")]
        public DateOnly? PassportIssueDate { get; set; }

        [Required]
        [Column("registration_address")]
        public string RegistrationAddress { get; set; } = string.Empty;

        [Column("actual_address")]
        public string? ActualAddress { get; set; }

        [Column("phone")]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // Навигационные свойства
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
        public virtual ICollection<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();
        public virtual ICollection<UtilityCharge> UtilityCharges { get; set; } = new List<UtilityCharge>();
        public virtual ICollection<SubsidyCalculation> SubsidyCalculations { get; set; } = new List<SubsidyCalculation>();
        public virtual ICollection<CompensationCalculation> CompensationCalculations { get; set; } = new List<CompensationCalculation>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        // Вычисляемое свойство для отображения в ComboBox
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}