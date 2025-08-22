using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventaryManagementSystem.Models.InventaryManagementSystem;

[Table("type_outputs")]
[Index("Code", Name = "type_outputs_code_key", IsUnique = true)]
public partial class TypeOutput
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("title")]
    [StringLength(15)]
    public string Title { get; set; } = null!;

    [Column("description")]
    [StringLength(50)]
    public string? Description { get; set; }

    [Column("code")]
    public short Code { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("OutputType")]
    public virtual ICollection<ProductOutput> ProductOutputs { get; set; } = new List<ProductOutput>();
}
