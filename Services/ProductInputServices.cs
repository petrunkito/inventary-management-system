using InventaryManagementSystem.Services.Result;
using InventaryManagementSystem.helpers.ProductInputHelpers;
using InventaryManagementSystem.Models.InventaryManagementSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace InventaryManagementSystem.Services
{
    public class ProductInputsService : IProductInputsService
    {
        private readonly ApplicationDbContext context;

        public ProductInputsService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<TypeInput>> GetTypeInputs()
        {
            List<TypeInput> typeInputs = await (from t in context.TypeInputs
                         select new TypeInput()
                         {
                             Title = t.Title,
                             Code = t.Code
                         }).ToListAsync();

            return typeInputs;
        }
        public async Task<List<ProductInputSelect>> GetProductInputs()
        {
            List<ProductInputSelect> inputs = await (
                from pi in context.ProductInputs
                join p in context.Products on pi.ProductId equals p.Id
                join ti in context.TypeInputs on pi.InputTypeId equals ti.Id
                select new ProductInputSelect()
                {
                    ProductInputId = pi.Id,
                    ProductName = p.Name,
                    TypeInput = ti.Title,
                    Amount = pi.Amount,
                    CostPrice = pi.CostPrice,
                    CreatedAt = pi.CreatedAt,
                    IsActive = pi.IsActive
                }
            ).ToListAsync();

            return inputs;
        }

        public async Task<Result<bool>> CreateProductInput(ProductInputCreate productInputCreate)
        {
            if (productInputCreate.ProductId is null)
                return Result<bool>.Failure("No has proporcionado ningùn producto");

            if (productInputCreate.InputType is null)
                return Result<bool>.Failure("No has proporcionado el tipo de entrada");

            if (productInputCreate.Amount is null)
                return Result<bool>.Failure("No has proporcionado la cantidad de la entrada");
            
            if (productInputCreate.CostPrice is null && InputTypeCode.Compra == (InputTypeCode)productInputCreate.InputType)
                return Result<bool>.Failure("No has proporcionado el tipo de entrada");

            Product? product = await (from p in context.Products
                                      where p.Id == productInputCreate.ProductId && p.IsActive
                                      select p).FirstOrDefaultAsync();

            if (product is null)
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
                ProductId = (Guid)productInputCreate.ProductId,
                InputTypeId = typeInput.Id,
                Amount = (int)productInputCreate.Amount,
                CostPrice = productInputCreate.CostPrice ?? 0,
                IsActive = true,
                CreatedAt = DateTime.Now,
            };
            product.Stock += productInput.Amount;
            context.ProductInputs.Add(productInput);
            await context.SaveChangesAsync();

            return Result<bool>.Ok(true);
        }

        public async Task<Result<bool>> DeactiveProductInput(Guid uuid)
        {
            ProductInput? input = (from pi in context.ProductInputs
                                   where pi.Id == uuid
                                   select pi).FirstOrDefault();
            if (input is null)
                return Result<bool>.Failure("No se encontro la entrada del producto.");

            if (input.IsActive is false)
                return Result<bool>.Failure("Esta entrada no se puede volver a activar.");


            DateTime createdAt = input.CreatedAt;
            DateTime now = DateTime.Now;
            if (createdAt.Year != now.Year || createdAt.Month != now.Month || createdAt.Day != now.Day)
                return Result<bool>.Failure("Solo puedes anular las entradas que se hicieron el mismo día.");

            Product? product = await context.Products.FirstOrDefaultAsync(x=>x.Id == input.ProductId);
            if (product is null)
                return Result<bool>.Failure("No se encontro el producto.");

            input.IsActive = false;
            product.Stock -= input.Amount;
            await context.SaveChangesAsync();
            return Result<bool>.Ok(true);

        }
    }


    public interface IProductInputsService
    {
        Task<List<TypeInput>> GetTypeInputs();
        Task<List<ProductInputSelect>> GetProductInputs();
        Task<Result<bool>> CreateProductInput(ProductInputCreate productInputCreate);
        Task<Result<bool>> DeactiveProductInput(Guid id);
    }
}
