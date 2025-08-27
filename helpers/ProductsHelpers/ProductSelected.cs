namespace InventaryManagementSystem.helpers.ProductsHelpers;

public class ViewProductSelected
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Amount { get; set; }
    public decimal Price { get; set; }
    public decimal Total
    {
        get
        {
            return Amount * Price;
        }
    }
    
}