using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SubsidySystem.Models;

/// <summary>
/// Фактические начисления за ЖКУ
/// </summary>
[Table("utility_charges")]
[Index("CitizenId", Name = "idx_utility_charges_citizen")]
[Index("Year", "Month", Name = "idx_utility_charges_period")]
[Index("CitizenId", "ServiceTypeId", "Year", "Month", Name = "idx_utility_charges_unique", IsUnique = true)]
public partial class UtilityCharge
{
    [Key]
    [Column("charge_id")]
    public int ChargeId { get; set; }

    [Column("citizen_id")]
    public int CitizenId { get; set; }

    [Column("service_type_id")]
    public int ServiceTypeId { get; set; }

    /// <summary>
    /// Год начисления
    /// </summary>
    [Column("year")]
    public int Year { get; set; }

    /// <summary>
    /// Месяц начисления
    /// </summary>
    [Column("month")]
    public int Month { get; set; }

    [Column("charge_amount")]
    [Precision(10, 2)]
    public decimal ChargeAmount { get; set; }

    [Column("consumption_volume")]
    [Precision(10, 2)]
    public decimal? ConsumptionVolume { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CitizenId")]
    [InverseProperty("UtilityCharges")]
    public virtual Citizen Citizen { get; set; } = null!;

    [ForeignKey("ServiceTypeId")]
    [InverseProperty("UtilityCharges")]
    public virtual UtilityServiceType ServiceType { get; set; } = null!;
}
