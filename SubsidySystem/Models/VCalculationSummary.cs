using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SubsidySystem.Models;

[Keyless]
public partial class VCalculationSummary
{
    [Column("type")]
    public string? Type { get; set; }

    [Column("citizen_id")]
    public int? CitizenId { get; set; }

    [Column("citizen_name")]
    public string? CitizenName { get; set; }

    [Column("year")]
    public int? Year { get; set; }

    [Column("month")]
    public int? Month { get; set; }

    [Column("amount")]
    [Precision(10, 2)]
    public decimal? Amount { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string? Status { get; set; }
}
