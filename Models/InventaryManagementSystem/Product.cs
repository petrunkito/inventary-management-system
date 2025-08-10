using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventaryManagementSystem.Models.InventaryManagementSystem;

[Table("products")]
public partial class Product
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [Column("supplier_id")]
    public Guid SupplierId { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(200)]
    public string? Description { get; set; }

    [Column("sale_price")]
    [Precision(10, 2)]
    public decimal SalePrice { get; set; }

    [Column("stock")]
    public int Stock { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<DepartureDetail> DepartureDetails { get; set; } = new List<DepartureDetail>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductInput> ProductInputs { get; set; } = new List<ProductInput>();

    [ForeignKey("SupplierId")]
    [InverseProperty("Products")]
    public virtual Supplier Supplier { get; set; } = null!;
}
