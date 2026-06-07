using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubsidySystem.Models
{
    [Table("standards")]
    public class Standard
    {
        [Key]
        [Column("standard_id")]
        public int StandardId { get; set; }

        [Required]
        [Column("standard_type")]
        [MaxLength(50)]
        public string StandardType { get; set; } = string.Empty;

        [Required]
        [Column("standard_value")]
        public decimal StandardValue { get; set; }

        [Column("territory_code")]
        [MaxLength(10)]
        public string? TerritoryCode { get; set; }

        [Required]
        [Column("valid_from", TypeName = "date")]
        public DateTime ValidFrom { get; set; }

        [Column("valid_to", TypeName = "date")]
        public DateTime? ValidTo { get; set; }  // nullable

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }  // nullable
    }
}