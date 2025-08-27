using InventaryManagementSystem.helpers.ProductOutputHelpers;
using InventaryManagementSystem.helpers.ProductsHelpers;
using InventaryManagementSystem.Models.InventaryManagementSystem;
using InventaryManagementSystem.Services.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Transactions;

namespace InventaryManagementSystem.Services;

public class ProductOutputsServices : IProductOutputsService
{
    private readonly ApplicationDbContext context;
    public ProductOutputsServices(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<List<TypeOutput>> GetTypeInputs()
    {
        List<TypeOutput> typeOutputs = await (from t in context.TypeOutputs
                                              select new TypeOutput()
                                              {
                                                  Title = t.Title,
                                                  Code = t.Code
                                              }).ToListAsync();

        return typeOutputs;
    }
    public async Task<List<ProductOutputSelect>> GetProductOutputs()
    {
        List<ProductOutputSelect> outputs = await (
            from p in context.ProductOutputs
            join c in context.Customers on p.CustomerId equals c.Id
            join o in context.TypeOutputs on p.OutputTypeId equals o.Id
            orderby p.CreatedAt ascending
            select new ProductOutputSelect
            {
                ProductOutputId = p.Id,
                CustomerName = c.Name,
                Amount = p.Amount,
                TypeOutput = o.Title,
                Total = p.TotalPrice,
                CreatedAt = p.CreatedAt,
                IsActive = p.IsActive
            }

        ).ToListAsync();
        return outputs;
    }
    public async Task<Result<bool>> CreateProductOutput(ProductOutputCreate productOutputCreate)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            if (productOutputCreate.CustomerId is null)
                return Result<bool>.Failure("Debes proporcionar algùn cliente");

            if (productOutputCreate.OutputTypeCode is null)
                return Result<bool>.Failure("Debes proporcionar el tipo de salida");

            if (productOutputCreate.SelectedProducts is null || !productOutputCreate.SelectedProducts.Any())
                return Result<bool>.Failure("Para la salida de productos debes proporcionar almenos un producto");


            Guid? customerId = await (from c in context.Customers
                                      where c.Id == productOutputCreate.CustomerId && c.IsActive
                                      select c.Id).FirstOrDefaultAsync();

            if (customerId is null)
                return Result<bool>.Failure("No se encontro al cliente");

            short code = (short)productOutputCreate.OutputTypeCode;
            TypeOutput? typeOutput = await (from t in context.TypeOutputs where t.Code == code select t).FirstOrDefaultAsync();
            if (typeOutput is null)
                return Result<bool>.Failure("Selecciona un tipo valido de salida de productos.");

            productOutputCreate.SelectedProducts = (from p in productOutputCreate.SelectedProducts
                                                    group p by p.ProductId into g
                                                    select new ProductSelected
                                                    {
                                                        ProductId = g.Key,
                                                        Amount = g.Sum(x => x.Amount)
                                                    }).ToList();

            List<Guid> ids = productOutputCreate.SelectedProducts.Select(x => x.ProductId).ToList();
            List<Product> products = await (
                from p in context.Products
                where ids.Contains(p.Id) && p.IsActive
                select p
            ).ToListAsync();

            ProductOutput productOutput = new ProductOutput()
            {
                OutputTypeId = typeOutput.Id,
                CustomerId = (Guid)customerId,
                Amount = 0,
                TotalPrice = 0,
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            context.ProductOutputs.Add(productOutput);
            await context.SaveChangesAsync();

            bool areThereProducts = false;
            int totalAmount = 0;
            decimal totalPrice = 0;

            //prueba enviando dos produtos del mismo por separado, para ver como los trata
            foreach (Product product in products)
            {
                ProductSelected? productSelected = productOutputCreate.SelectedProducts.FirstOrDefault(x => x.ProductId == product.Id);
                if (productSelected is null) continue;
                if (product.Stock < productSelected.Amount) continue;

                DepartureDetail departureDetail = new DepartureDetail()
                {
                    ProductId = product.Id,
                    ProductOutputId = productOutput.Id,
                    Amount = productSelected.Amount,
                    SalePrice = product.SalePrice,
                    Total = productSelected.Amount * product.SalePrice,
                    IsActive = true
                };
                context.DepartureDetails.Add(departureDetail);
                totalAmount += productSelected.Amount;
                totalPrice += product.SalePrice * productSelected.Amount;

                product.Stock -= productSelected.Amount;
                areThereProducts = true;
            }
            await context.SaveChangesAsync();

            if (!areThereProducts)
                return Result<bool>.Failure("Para hacer una salida de productos, debe haber al menos un producto, esto pudo ocurrir que los productos no estan activos o no hay suficiente en stock");

            productOutput.Amount = totalAmount;
            productOutput.TotalPrice = totalPrice;
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<bool>.Ok(true);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Result<bool>.Failure("ocurrio un problema al tratar de crear la salida");
        }
    }
    public async Task<Result<bool>> DeactiveProductOutput(Guid id)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            ProductOutput? output = (from pi in context.ProductOutputs
                                     where pi.Id == id
                                     select pi).FirstOrDefault();
            if (output is null)
                return Result<bool>.Failure("No se encontro la salida.");

            if (!output.IsActive)
                return Result<bool>.Failure("Esta salida no se puede volver a activar.");


            DateTime createdAt = output.CreatedAt;
            DateTime now = DateTime.Now;
            if (createdAt.Year != now.Year || createdAt.Month != now.Month || createdAt.Day != now.Day)
                return Result<bool>.Failure("Solo puedes anular las salidas que se hicieron el mismo día.");


            List<DepartureDetail> productDetails = await (from p in context.DepartureDetails
                                                          where p.ProductOutputId == output.Id
                                                          select p).ToListAsync();
            if (!productDetails.Any())
                return Result<bool>.Failure("No se encontraron los detalles de la salida.");

            output.IsActive = false;
            foreach (DepartureDetail productDetail in productDetails)
                productDetail.IsActive = false;

            List<Guid> ids = productDetails.Select(x => x.ProductId).ToList();
            List<Product> products = await (from p in context.Products
                                            where ids.Contains(p.Id)
                                            select p).ToListAsync();

            foreach (Product product in products)
            {
                DepartureDetail? departureDetail = productDetails.FirstOrDefault(x => x.ProductId == product.Id);
                if (departureDetail is null) continue;
                product.Stock += departureDetail.Amount;
            }
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Result<bool>.Ok(true);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Result<bool>.Ok(false);

        }

    }
}

public interface IProductOutputsService
{
    Task<List<TypeOutput>> GetTypeInputs();

    Task<List<ProductOutputSelect>> GetProductOutputs();
    Task<Result<bool>> CreateProductOutput(ProductOutputCreate productOutputCreate);
    Task<Result<bool>> DeactiveProductOutput(Guid id);


}