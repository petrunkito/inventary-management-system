using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventaryManagementSystem.Models.InventaryManagementSystem;

[Table("type_outputs")]
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

    [InverseProperty("OutputType")]
    public virtual ICollection<ProductOutput> ProductOutputs { get; set; } = new List<ProductOutput>();
}
