using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventaryManagementSystem.Models.InventaryManagementSystem;

[Table("suppliers")]
[Index("Name", Name = "name_unique", IsUnique = true)]
public partial class Supplier
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("number_phone")]
    [StringLength(9)]
    public string? NumberPhone { get; set; }

    [Column("address")]
    [StringLength(150)]
    public string? Address { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("Supplier")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
