using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventaryManagementSystem.Models.InventaryManagementSystem;

[Table("type_inputs")]
[Index("Code", Name = "unique_code", IsUnique = true)]
public partial class TypeInput
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

    [InverseProperty("InputType")]
    public virtual ICollection<ProductInput> ProductInputs { get; set; } = new List<ProductInput>();
}
