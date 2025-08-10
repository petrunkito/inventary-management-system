namespace InventaryManagementSystem.helpers.ProductsHelpers;

public class ProductDetail
{
    
    public Guid? CategoryId { get; set; }
    public Guid? SupplierId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    public decimal? CostPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public int? Stock { get; set; }
}