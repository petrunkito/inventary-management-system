using InventaryManagementSystem.helpers.ProductsHelpers;
using InventaryManagementSystem.Services;
using Microsoft.AspNetCore.Components;
using InventaryManagementSystem.Components.Dialog;
using MudBlazor;
using InventaryManagementSystem.Services.Result;
using InventaryManagementSystem.Models.InventaryManagementSystem;

namespace InventaryManagementSystem.Components.Pages;

public partial class Products : ComponentBase
{

    [Inject]
    protected IProductsService ProductsService { get; set; } = default!;
    [Inject]
    protected IDialogService DialogService { get; set; } = default!;

    private List<SelectProductDetail> products = default!;

    protected override async Task OnInitializedAsync()
    {
        await FillProducts();
    }

    private async Task FillProducts()
    {
        products = await ProductsService.GetAllProducts();
    }
    private async Task CreateProduct()
    {
        string titleDialog = "Crear producto";
        var dialog = await DialogService.ShowAsync<CreateProductDialog>(titleDialog);
        var result = await dialog.Result;

        if (result is null) return;
        if (result?.Canceled is true) return;

        ProductDetail? detailProduct = (ProductDetail?)result?.Data;
        if (detailProduct is null) return;


        Guid? categoryId = detailProduct.CategoryId;
        Guid? supplierId = detailProduct.SupplierId;
        string? name = detailProduct.Name?.Trim();
        string? description = detailProduct.Description?.Trim();
        int stock = detailProduct.Stock ?? 0;
        decimal? costPrice = detailProduct.CostPrice;
        decimal? salePrice = detailProduct.SalePrice;

        if (categoryId is null)
        {
            await DialogService.ShowMessageBox("", "Antes de crear un producto, debes elegir una categoría", yesText: "Comprendo");
            return;
        }

        if (supplierId is null)
        {
            await DialogService.ShowMessageBox("", "Antes de crear un producto, debes elegir un proveedor", yesText: "Comprendo");
            return;
        }
        if (string.IsNullOrEmpty(name))
        {
            await DialogService.ShowMessageBox("", "El nombre del producto no puede estar vacío.", yesText: "Comprendo");
            return;
        }
        if (stock < 0)
        {
            await DialogService.ShowMessageBox("", "La cantidad no puede ser negativa.", yesText: "Comprendo");
            return;
        }

        if (costPrice <= 0)
        {
            await DialogService.ShowMessageBox("", "El precio de costo debe ser mayor a 0", yesText: "Comprendo");
            return;
        }
        if (salePrice < costPrice)
        {
            await DialogService.ShowMessageBox("", "El precio de venta debe ser mayor al precio de costo", yesText: "Comprendo");
            return;
        }

        Result<Product> resultCreation = await ProductsService.CreateProduct(new ProductDetail
        {
            CategoryId = categoryId,
            SupplierId = supplierId,
            Name = name,
            Description = description,
            CostPrice = costPrice,
            SalePrice = salePrice,
            Stock = stock

        });

        if (resultCreation.Success is false)
        {
            await DialogService.ShowMessageBox("", resultCreation.Error ?? "Ocurrió un problema al tratar de crear el producto.", yesText: "Comprendo");
            return;
        }

        await FillProducts();
    }

    //!quedamos en la construccion del metodo editProduct
    private async Task EditProduct(Guid id)
    {
        string titleDialog = "Editar producto";

        SelectProductDetail? product = products.FirstOrDefault(x => x.Id == id);
        if (product is null)
        {
            await DialogService.ShowMessageBox("", "No se encontró el producto.", yesText: "Comprendo");
            return;
        }

        DialogParameters parameters = new DialogParameters<EditAndShowProductDialog>
            {
                {x=> x.SelectProductDetail, new SelectProductDetail{
                    ProductName = product.ProductName,
                    Description = product.Description,
                    CategoryName = product.CategoryName,
                    SupplierName = product.SupplierName,
                    SalePrice = product.SalePrice,
                }}
            };
        var dialog = await DialogService.ShowAsync<EditAndShowProductDialog>(titleDialog, parameters);
        var result = await dialog.Result;

        if (result is null) return;
        if (result?.Canceled is true) return;

        ProductDetail? detailProduct = (ProductDetail?)result!.Data;

        if (detailProduct is null) return;

        Guid? categoryId = detailProduct.CategoryId;
        Guid? supplierId = detailProduct.SupplierId;
        string? name = detailProduct.Name?.Trim();
        string? description = detailProduct.Description?.Trim();
        int stock = detailProduct.Stock ?? 0;

        if (categoryId is null)
        {
            await DialogService.ShowMessageBox("", "Antes de editar un producto, debes elegir una categoría", yesText: "Comprendo");
            return;
        }
        if (supplierId is null)
        {
            await DialogService.ShowMessageBox("", "Antes de editar un producto, debes elegir un proveedor", yesText: "Comprendo");
            return;
        }
        if (string.IsNullOrEmpty(name))
        {
            await DialogService.ShowMessageBox("", "El nombre del producto no puede estar vacío.", yesText: "Comprendo");
            return;
        }
        if (stock < 0)
        {
            await DialogService.ShowMessageBox("", "La cantidad no puede ser negativa.", yesText: "Comprendo");
            return;
        }

        Result<Product> resultEdition = await ProductsService.UpdateProduct(id, new ProductDetail
        {
            CategoryId = categoryId,
            SupplierId = supplierId,
            Name = name,
            Description = description,
            Stock = stock,
            SalePrice = detailProduct.SalePrice
        });

        if (resultEdition.Success is false)
        {
            await DialogService.ShowMessageBox("", resultEdition.Error, yesText: "Comprendo");
            return;
        }

        await FillProducts();
    }
    private async Task ShowProductDetail(Guid id)
    {
         string titleDialog = "Detalles del producto";

        SelectProductDetail? product = products.FirstOrDefault(x => x.Id == id);
        if (product is null)
        {
            await DialogService.ShowMessageBox("", "No se encontró el producto.", yesText: "Comprendo");
            return;
        }

        DialogParameters parameters = new DialogParameters<EditAndShowProductDialog>
            {
                {x=> x.SelectProductDetail, new SelectProductDetail{
                    ProductName = product.ProductName,
                    Description = product.Description,
                    CategoryName = product.CategoryName,
                    SupplierName = product.SupplierName,
                    SalePrice = product.SalePrice,
                    Stock = product.Stock,
                }},
                {x=> x.IsEditProduct, false}
            };
        await DialogService.ShowAsync<EditAndShowProductDialog>(titleDialog, parameters);
    }
    private async Task DeactiveAndActivateProduct(Guid id)
    {
        SelectProductDetail? product = products.FirstOrDefault(x => x.Id == id);
        if (product is null)
        {
            await DialogService.ShowMessageBox("", "No se encontró el producto.", yesText: "Comprendo");
            return;
        }

        string message = product.IsActive ? "¿Desea desactivar este producto?" : "¿Desea activar este producto?";
        bool? result = await DialogService.ShowMessageBox("", message, yesText: "Sí", cancelText: "No");
        if (result is null) return;

        Result<bool> resultDeactiveActivate = await ProductsService.DeactivateAndActivateProduct(id);

        if (resultDeactiveActivate.Success is false)
        {
            await DialogService.ShowMessageBox("", resultDeactiveActivate.Error, yesText: "Comprendo");
            return;
        }

        product.IsActive = resultDeactiveActivate.Data;
    }
}