namespace InventaryManagementSystem.helpers.ProductInputHelpers
{
    public class ProductInputCreate
    {
        public Guid ProductId { get; set; }
        public TypeCodeInput InputType { get; set; }
        public int Amount { get; set; }
        public decimal CostPrice { get; set; }
    }


    public enum TypeCodeInput
    {
        Compra,
        Devolucion
    }
}

