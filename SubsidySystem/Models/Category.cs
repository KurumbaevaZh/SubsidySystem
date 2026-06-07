using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SubsidySystem.Models;

/// <summary>
/// Категории граждан-льготников
/// </summary>
[Table("categories")]
public partial class Category
{
    /// <summary>
    /// Уникальный идентификатор категории
    /// </summary>
    [Key]
    [Column("category_id")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Наименование категории граждан
    /// </summary>
    [Column("category_name")]
    [StringLength(100)]
    public string CategoryName { get; set; } = null!;

    /// <summary>
    /// Размер скидки на оплату ЖКУ в процентах
    /// </summary>
    [Column("discount_rate")]
    [Precision(5, 2)]
    public decimal? DiscountRate { get; set; }

    /// <summary>
    /// Описание условий предоставления льготы
    /// </summary>
    [Column("eligibility_conditions")]
    public string? EligibilityConditions { get; set; }

    /// <summary>
    /// Наименование документа, устанавливающего льготу
    /// </summary>
    [Column("regulatory_act")]
    [StringLength(200)]
    public string? RegulatoryAct { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Citizen> Citizens { get; set; } = new List<Citizen>();

    [InverseProperty("Category")]
    public virtual ICollection<CompensationCalculation> CompensationCalculations { get; set; } = new List<CompensationCalculation>();
}
