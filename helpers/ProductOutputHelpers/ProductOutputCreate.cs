namespace InventaryManagementSystem.helpers.ProductOutputHelpers;

public class ProductOutputCreate
{
    public Guid? CustomerId { get; set; }
    public OutputTypeCode? OutputTypeCode { get; set; }
    public List<ProductSelected>? SelectedProducts { get; set; }
}

public class ProductSelected
{
    public Guid ProductId { get; set; }
    public int Amount { get; set; }
}

public enum OutputTypeCode
{
    Venta,
    Perdida
}