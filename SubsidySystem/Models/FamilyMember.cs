using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SubsidySystem.Models;

/// <summary>
/// Члены семьи заявителя
/// </summary>
[Table("family_members")]
[Index("CitizenId", Name = "idx_family_members_citizen")]
public partial class FamilyMember
{
    [Key]
    [Column("member_id")]
    public int MemberId { get; set; }

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
    /// Степень родства с заявителем
    /// </summary>
    [Column("relationship")]
    [StringLength(30)]
    public string Relationship { get; set; } = null!;

    /// <summary>
    /// Признак обучения (для учета иждивенцев)
    /// </summary>
    [Column("is_student")]
    public bool? IsStudent { get; set; }

    [Column("snils")]
    [StringLength(14)]
    public string? Snils { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "timestamp without time zone")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CitizenId")]
    [InverseProperty("FamilyMembers")]
    public virtual Citizen Citizen { get; set; } = null!;

    [InverseProperty("Member")]
    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
}
