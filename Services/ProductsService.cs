using InventaryManagementSystem.Models.InventaryManagementSystem;
using InventaryManagementSystem.Services.Result;
using InventaryManagementSystem.helpers.ProductsHelpers;
using Microsoft.EntityFrameworkCore;
using InventaryManagementSystem.helpers.ProductInputHelpers;

namespace InventaryManagementSystem.Services;

public class ProductsService : IProductsService
{
    private readonly ApplicationDbContext context;
    private readonly IProductInputsService productInputsService;

    public ProductsService(ApplicationDbContext context, IProductInputsService productInputsService)
    {
        this.context = context;
        this.productInputsService = productInputsService;
    }

    public async Task<Result<SelectProductDetail>> GetProductById(Guid productId)
    {
        SelectProductDetail? product = await (from p in context.Products
                                                    join c in context.Categories on p.CategoryId equals c.Id
                                                    join s in context.Suppliers on p.SupplierId equals s.Id
                                                    orderby p.CreatedAt ascending
                                                    select new SelectProductDetail()
                                                    {
                                                        Id = p.Id,
                                                        ProductName = p.Name,
                                                        CategoryName = c.Title,
                                                        SupplierName = s.Name,
                                                        Description = p.Description ?? string.Empty,
                                                        SalePrice = p.SalePrice,
                                                        Stock = p.Stock,
                                                        IsActive = p.IsActive
                                                    }).FirstOrDefaultAsync();
       
        if (product is null)
            return Result<SelectProductDetail>.Failure("No se encontro el producto especificado");

        return Result<SelectProductDetail>.Ok(product);
    }

    public async Task<List<SelectProductDetail>> GetAllProducts()
    {

        List<SelectProductDetail> products = await (from p in context.Products
                                                    join c in context.Categories on p.CategoryId equals c.Id
                                                    join s in context.Suppliers on p.SupplierId equals s.Id
                                                    orderby p.CreatedAt ascending
                                                    select new SelectProductDetail()
                                                    {
                                                        Id = p.Id,
                                                        ProductName = p.Name,
                                                        CategoryName = c.Title,
                                                        SupplierName = s.Name,
                                                        Description = p.Description ?? string.Empty,
                                                        SalePrice = p.SalePrice,
                                                        Stock = p.Stock,
                                                        IsActive = p.IsActive
                                                    }).ToListAsync();
        return products;
    }

    public async Task<Result<Product>> CreateProduct(ProductDetail createProductDetail)
    {
        Guid? categoryId = createProductDetail.CategoryId;
        if (categoryId is null)
            return Result<Product>.Failure("El ID de la categoría no puede estar vacío");

        Guid? supplierId = createProductDetail.SupplierId;
        if (supplierId is null)
            return Result<Product>.Failure("El ID del proveedor no puede estar vacío");

        if (string.IsNullOrEmpty(createProductDetail.Name?.Trim()))
            return Result<Product>.Failure("El nombre del producto no puede estar vacío");

        if (createProductDetail.SalePrice is null || createProductDetail.SalePrice <= 0)
            return Result<Product>.Failure("El precio de venta debe ser mayor a cero");

        if (createProductDetail.Stock is null || createProductDetail.Stock < 0 )
            return Result<Product>.Failure("La cantidad no puede ser negativa");

        if (createProductDetail.CostPrice is null ||createProductDetail.CostPrice <= 0)
            return Result<Product>.Failure("El precio de costo debe ser mayor a cero");

        if (createProductDetail.SalePrice < createProductDetail.CostPrice)
            return Result<Product>.Failure("El precio de venta debe ser mayor o igual al precio de costo");

        Category? category = await context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        if (category is null)
            return Result<Product>.Failure("No se encontro la categoría especificada");

        if (category.IsActive is false)
            return Result<Product>.Failure("La categoría que seleccionaste está inactiva, selecciona otra");

        Supplier? supplier = await context.Suppliers.FirstOrDefaultAsync(s => s.Id == supplierId);
        if (supplier is null)
            return Result<Product>.Failure("No se encontro el proveedor especificado");

        if (supplier.IsActive is false)
            return Result<Product>.Failure("El proveedor que seleccionaste está inactivo, selecciona otro");

        string productName = createProductDetail.Name.Trim();
        bool productExists = await context.Products.AnyAsync(p => p.Name == productName);
        if (productExists)
            return Result<Product>.Failure("El nombre del producto ya existe, elige otro");

        Product newProduct = new()
        {
            CategoryId = categoryId.Value,
            SupplierId = supplierId.Value,
            Name = createProductDetail.Name.Trim(),
            Description = createProductDetail.Description?.Trim(),
            SalePrice = (decimal)createProductDetail.SalePrice,
            Stock = (int)createProductDetail.Stock,
            CreatedAt = DateTime.Now,
            IsActive = true
        };

        
        context.Products.Add(newProduct);
        await context.SaveChangesAsync();
        
        Result<bool> result = await productInputsService.CreateProductInput(new ProductInputCreate
        {
            ProductId = newProduct.Id,
            InputType = TypeCodeInput.Compra,
            Amount = (int)createProductDetail.Stock,
            CostPrice = (decimal)createProductDetail.CostPrice
        });

        return Result<Product>.Ok(newProduct);
    }

    public async Task<Result<bool>> DeactivateAndActivateProduct(Guid uuid)
    {
        Product? productExists = await context.Products.FirstOrDefaultAsync(p => p.Id == uuid);
        if (productExists is null)
            return Result<bool>.Failure("No se encontro el producto especificado");

        productExists.IsActive = !productExists.IsActive;

        await context.SaveChangesAsync();
        return Result<bool>.Ok(productExists.IsActive);
    }

    public async Task<Result<Product>> UpdateProduct(Guid uuid, ProductDetail updateProductDetail)
    {
        if (updateProductDetail.CategoryId is null &&
           updateProductDetail.SupplierId is null &&
           string.IsNullOrEmpty(updateProductDetail.Name?.Trim()) &&
           string.IsNullOrEmpty(updateProductDetail.Description?.Trim()) &&
           updateProductDetail.SalePrice is null)
            return Result<Product>.Failure("Debes proporcionar algún dato para actualizar al producto");

        Product? product = await context.Products.FirstOrDefaultAsync(x => x.Id == uuid);
        if (product is null)
            return Result<Product>.Failure("El producto seleccionado no se encontro");

        Guid? categoryId = updateProductDetail.CategoryId;
        if (categoryId is not null)
        {
            bool categoryExist = await context.Categories.AnyAsync(x => x.Id == categoryId && x.IsActive);
            if (categoryExist is false)
                return Result<Product>.Failure("No se encontró la categoría o está inactiva");


            product.CategoryId = (Guid)categoryId;

        }

        Guid? supplierId = updateProductDetail.SupplierId;
        if (supplierId is not null)
        {
            bool supplierExist = await context.Suppliers.AnyAsync(x => x.Id == supplierId && x.IsActive);
            if (supplierExist is false)
                return Result<Product>.Failure("No se encontró el proveedor o está inactivo");

            product.SupplierId = (Guid)supplierId;
        }

        string? name = updateProductDetail.Name?.Trim();
        if (name is not null && name.Length > 0) product.Name = name;

        string? description = updateProductDetail.Description?.Trim();
        if (description is not null && description.Length > 0) product.Description = description;

        decimal? salePrice = updateProductDetail.SalePrice;
        if (salePrice is not null && salePrice > 0) product.SalePrice = (decimal)salePrice;


        await context.SaveChangesAsync();
        return Result<Product>.Ok(product);
    }
}

public interface IProductsService
{
    Task<Result<SelectProductDetail>> GetProductById(Guid productId);
    Task<List<SelectProductDetail>> GetAllProducts();
    Task<Result<Product>> CreateProduct(ProductDetail createProductDetail);
    Task<Result<bool>> DeactivateAndActivateProduct(Guid uuid);
    Task<Result<Product>> UpdateProduct(Guid uuid, ProductDetail updateProductDetail);
}


 