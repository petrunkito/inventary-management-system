using Microsoft.AspNetCore.Mvc;

namespace InventaryManagementSystem.helpers.ProductInputHelpers;

public class ProductInputSelect
{
    public Guid ProductInputId { get; set; }
    public string ProductName { get; set; } = default!;
    public string TypeInput { get; set; } = default!;//el texto del campo 'title' de la tabla 'type_inputs'
    public int Amount { get; set; }
    public decimal CostPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}