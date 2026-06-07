using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SubsidySystem.Models;

/// <summary>
/// Классификатор услуг ЖКУ
/// </summary>
[Table("utility_service_types")]
public partial class UtilityServiceType
{
    [Key]
    [Column("service_type_id")]
    public int ServiceTypeId { get; set; }

    [Column("service_type_name")]
    [StringLength(100)]
    public string ServiceTypeName { get; set; } = null!;

    /// <summary>
    /// Единица измерения потребления
    /// </summary>
    [Column("unit_of_measure")]
    [StringLength(10)]
    public string? UnitOfMeasure { get; set; }

    [InverseProperty("ServiceType")]
    public virtual ICollection<UtilityCharge> UtilityCharges { get; set; } = new List<UtilityCharge>();
}
