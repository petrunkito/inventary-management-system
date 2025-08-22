namespace InventaryManagementSystem.helpers.ProductOutputHelpers;



public class ProductOutputSelect
{
    public Guid ProductOutputId { get; set; }
    public string CustomerName { get; set; } = default!;
    public int Amount { get; set; }
    public string TypeOutput { get; set; } = default!;
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

}