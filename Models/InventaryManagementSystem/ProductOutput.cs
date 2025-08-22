using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventaryManagementSystem.Models.InventaryManagementSystem;

[Table("product_outputs")]
public partial class ProductOutput
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("output_type_id")]
    public Guid OutputTypeId { get; set; }

    [Column("customer_id")]
    public Guid CustomerId { get; set; }

    [Column("amount")]
    public int Amount { get; set; }

    [Column("total_price")]
    [Precision(10, 2)]
    public decimal TotalPrice { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("ProductOutputs")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("ProductOutput")]
    public virtual ICollection<DepartureDetail> DepartureDetails { get; set; } = new List<DepartureDetail>();

    [ForeignKey("OutputTypeId")]
    [InverseProperty("ProductOutputs")]
    public virtual TypeOutput OutputType { get; set; } = null!;
}
