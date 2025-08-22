using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventaryManagementSystem.Models.InventaryManagementSystem;

[Table("product_inputs")]
public partial class ProductInput
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("product_id")]
    public Guid ProductId { get; set; }

    [Column("input_type_id")]
    public Guid InputTypeId { get; set; }

    [Column("amount")]
    public int Amount { get; set; }

    [Column("cost_price")]
    [Precision(10, 2)]
    public decimal CostPrice { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("InputTypeId")]
    [InverseProperty("ProductInputs")]
    public virtual TypeInput InputType { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductInputs")]
    public virtual Product Product { get; set; } = null!;
}
