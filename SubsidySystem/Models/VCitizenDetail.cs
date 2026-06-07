using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SubsidySystem.Models;

[Keyless]
public partial class VCitizenDetail
{
    [Column("citizen_id")]
    public int? CitizenId { get; set; }

    [Column("full_name")]
    public string? FullName { get; set; }

    [Column("birth_date")]
    public DateOnly? BirthDate { get; set; }

    [Column("snils")]
    [StringLength(14)]
    public string? Snils { get; set; }

    [Column("registration_address")]
    public string? RegistrationAddress { get; set; }

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Column("льготная_категория")]
    [StringLength(100)]
    public string? ЛьготнаяКатегория { get; set; }

    [Column("размер_скидки")]
    [Precision(5, 2)]
    public decimal? РазмерСкидки { get; set; }
}
