using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SubsidySystem.Models;

/// <summary>
/// Сведения о доходах членов семьи
/// </summary>
[Table("incomes")]
[Index("MemberId", Name = "idx_incomes_member")]
[Index("PeriodStart", "PeriodEnd", Name = "idx_incomes_period")]
public partial class Income
{
    [Key]
    [Column("income_id")]
    public int IncomeId { get; set; }

    [Column("member_id")]
    public int MemberId { get; set; }

    [Column("income_type_id")]
    public int IncomeTypeId { get; set; }

    /// <summary>
    /// Сумма дохода
    /// </summary>
    [Column("amount")]
    [Precision(10, 2)]
    public decimal Amount { get; set; }

    /// <summary>
    /// Начало периода, за который получен доход
    /// </summary>
    [Column("period_start")]
    public DateOnly PeriodStart { get; set; }

    /// <summary>
    /// Окончание периода, за который получен доход
    /// </summary>
    [Column("period_end")]
    public DateOnly PeriodEnd { get; set; }

    [Column("document_name")]
    [StringLength(100)]
    public string? DocumentName { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("IncomeTypeId")]
    [InverseProperty("Incomes")]
    public virtual IncomeType IncomeType { get; set; } = null!;

    [ForeignKey("MemberId")]
    [InverseProperty("Incomes")]
    public virtual FamilyMember Member { get; set; } = null!;
}
