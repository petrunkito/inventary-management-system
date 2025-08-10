using InventaryManagementSystem.Services.Result;
using InventaryManagementSystem.helpers.ProductInputHelpers;
using InventaryManagementSystem.Models.InventaryManagementSystem;
using Microsoft.EntityFrameworkCore;


namespace InventaryManagementSystem.Services
{
    public class ProductInputsService : IProductInputsService
    {
        private readonly ApplicationDbContext context;

        public ProductInputsService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Result<bool>> CreateProductInput(ProductInputCreate productInputCreate)
        {
            Guid? result = await (from p in context.Products
                                  where p.Id == productInputCreate.ProductId && p.IsActive
                                  select p.Id).FirstOrDefaultAsync();

            if (result is null)
                return Result<bool>.Failure("No se encontro el producto especificado");

            TypeInput? typeInput = await (from t in context.TypeInputs
                                          where t.Code == (int)productInputCreate.InputType &&
                                          t.IsActive
                                          select t).FirstOrDefaultAsync();
            if (typeInput is null)
                return Result<bool>.Failure("No se encontro el tipo de entrada especificado");
            // Crear la entrada de producto
            ProductInput productInput = new ProductInput
            {
                ProductId = productInputCreate.ProductId,
                InputTypeId = typeInput.Id,
                Amount = productInputCreate.Amount,
                CostPrice = productInputCreate.CostPrice,
                IsActive = true,
                CreatedAt = DateTime.Now,
            };

            context.ProductInputs.Add(productInput);
            context.SaveChanges();

            return Result<bool>.Ok(true);
        }
    }


    public interface IProductInputsService
    {
        Task<Result<bool>> CreateProductInput(ProductInputCreate productInputCreate);
    }
}
