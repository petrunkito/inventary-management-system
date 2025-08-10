
namespace InventaryManagementSystem.helpers.ProductsHelpers;

public class SelectProductDetail
{
    public Guid Id { get; set; } = default!;
    public string ProductName { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public string SupplierName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal SalePrice { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }


}