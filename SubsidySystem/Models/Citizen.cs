using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SubsidySystem.Models;

/// <summary>
/// Персональные данные заявителей
/// </summary>
[Table("citizens")]
[Index("LastName", "FirstName", Name = "idx_citizens_name")]
[Index("Snils", Name = "idx_citizens_snils")]
public partial class Citizen
{
    /// <summary>
    /// Уникальный идентификатор гражданина
    /// </summary>
    [Key]
    [Column("citizen_id")]
    public int CitizenId { get; set; }

    [Column("last_name")]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Column("first_name")]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Column("middle_name")]
    [StringLength(50)]
    public string? MiddleName { get; set; }

    [Column("birth_date")]
    public DateOnly BirthDate { get; set; }

    /// <summary>
    /// Страховой номер индивидуального лицевого счета
    /// </summary>
    [Column("snils")]
    [StringLength(14)]
    public string? Snils { get; set; }

    [Column("passport_series")]
    [StringLength(4)]
    public string? PassportSeries { get; set; }

    [Column("passport_number")]
    [StringLength(6)]
    public string? PassportNumber { get; set; }

    [Column("passport_issuer")]
    [StringLength(200)]
    public string? PassportIssuer { get; set; }

    [Column("passport_issue_date")]
    public DateOnly? PassportIssueDate { get; set; }

    [Column("registration_address")]
    public string RegistrationAddress { get; set; } = null!;

    [Column("actual_address")]
    public string? ActualAddress { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// Идентификатор категории льготности (внешний ключ)
    /// </summary>
    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Citizens")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Citizen")]
    public virtual ICollection<CompensationCalculation> CompensationCalculations { get; set; } = new List<CompensationCalculation>();

    [InverseProperty("Citizen")]
    public virtual ICollection<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();

    [InverseProperty("Citizen")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [InverseProperty("Citizen")]
    public virtual ICollection<SubsidyCalculation> SubsidyCalculations { get; set; } = new List<SubsidyCalculation>();

    [InverseProperty("Citizen")]
    public virtual ICollection<UtilityCharge> UtilityCharges { get; set; } = new List<UtilityCharge>();
}
