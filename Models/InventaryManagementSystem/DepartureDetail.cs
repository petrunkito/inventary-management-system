using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventaryManagementSystem.Models.InventaryManagementSystem;

[Table("departure_details")]
public partial class DepartureDetail
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("product_id")]
    public Guid ProductId { get; set; }

    [Column("product_output_id")]
    public Guid ProductOutputId { get; set; }

    [Column("amount")]
    public int Amount { get; set; }

    [Column("sale_price")]
    [Precision(10, 2)]
    public decimal SalePrice { get; set; }

    [Column("total")]
    [Precision(10, 2)]
    public decimal Total { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("DepartureDetails")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("ProductOutputId")]
    [InverseProperty("DepartureDetails")]
    public virtual ProductOutput ProductOutput { get; set; } = null!;
}
