using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SubsidySystem.Models;

/// <summary>
/// Классификатор видов доходов
/// </summary>
[Table("income_types")]
public partial class IncomeType
{
    [Key]
    [Column("income_type_id")]
    public int IncomeTypeId { get; set; }

    [Column("income_type_name")]
    [StringLength(100)]
    public string IncomeTypeName { get; set; } = null!;

    /// <summary>
    /// Признак учета при расчете среднедушевого дохода
    /// </summary>
    [Column("is_considered")]
    public bool? IsConsidered { get; set; }

    [InverseProperty("IncomeType")]
    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
}
