using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventaryManagementSystem.Models.InventaryManagementSystem;

[Table("categories")]
[Index("Title", Name = "title_unique", IsUnique = true)]
public partial class Category
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("title")]
    [StringLength(25)]
    public string Title { get; set; } = null!;

    [Column("description")]
    [StringLength(80)]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
