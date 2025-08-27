using InventaryManagementSystem.helpers.ProductsHelpers;
using InventaryManagementSystem.Services;
using Microsoft.AspNetCore.Components;
using InventaryManagementSystem.Components.Dialog;
using MudBlazor;
using InventaryManagementSystem.Services.Result;
using InventaryManagementSystem.Models.InventaryManagementSystem;
using InventaryManagementSystem.helpers.ProductInputHelpers;

namespace InventaryManagementSystem.Components.Pages;

public partial class Inputs : ComponentBase
{

    [Inject]
    protected IProductInputsService ProductInputService { get; set; } = default!;
    [Inject]
    protected IDialogService DialogService { get; set; } = default!;

    private List<ProductInputSelect> inputs = default!;

    protected override async Task OnInitializedAsync()
    {
        await FillInputProducts();
    }

    private async Task FillInputProducts()
    {
        inputs = await ProductInputService.GetProductInputs();
    }
    private async Task CreateInputProduct()
    {
        string titleDialog = "Crear entrada";
        var dialog = await DialogService.ShowAsync<CreateInputDialog>(titleDialog);
        var result = await dialog.Result;

        if (result is null) return;
        if (result?.Canceled is true) return;

        ProductInputCreate? detailProduct = (ProductInputCreate?)result?.Data;
        if (detailProduct is null) return;


        Guid? productId = detailProduct.ProductId;
        InputTypeCode? inputTypeCode = detailProduct.InputType;
        int amount = detailProduct.Amount ?? 0;
        decimal costPrice = detailProduct.CostPrice ?? 0;
        
        if (productId is null)
        {
            await DialogService.ShowMessageBox("", "Antes de crear una entrada, debes elegir una producto", yesText: "Comprendo");
            return;
        }

        if (inputTypeCode is null)
        {
            await DialogService.ShowMessageBox("", "Antes de crear una entrada, debes elegir el tipo de entrada", yesText: "Comprendo");
            return;
        }
        if (amount == 0)
        {
            await DialogService.ShowMessageBox("", "Antes de crear una entrada, la cantidad a ingresar debe ser mayor a 0", yesText: "Comprendo");
            return;
        }
        if (costPrice == 0 && InputTypeCode.Compra == inputTypeCode)
        {
            await DialogService.ShowMessageBox("", "Antes de crear una entrada, el precio de costo debe ser mayor a 0", yesText: "Comprendo");
            return;
        }

        Result<bool> resultCreation = await ProductInputService.CreateProductInput(new ProductInputCreate
        {
            ProductId = productId,
            InputType = inputTypeCode,
            Amount = amount,
            CostPrice = costPrice
        });

        if (resultCreation.Success is false)
        {
            await DialogService.ShowMessageBox("", resultCreation.Error ?? "Ocurrió un problema al tratar de crear la entrada para este producto.", yesText: "Comprendo");
            return;
        }

        await FillInputProducts();
    }
    private async Task DeactiveInputProduct(Guid id)
    {
        ProductInputSelect? input = inputs.FirstOrDefault(x => x.ProductInputId == id);
        if (input is null)
        {
            await DialogService.ShowMessageBox("", "No se encontró la entrada.", yesText: "Comprendo");
            return;
        }
        

        MarkupString message = (MarkupString)"¿Desea desactivar esta entrada?<br/>Este cambio no es reversible";
        bool? result = await DialogService.ShowMessageBox("", message, yesText: "Sí", cancelText: "No");
        if (result is null) return;

        Result<bool> resultDeactive = await ProductInputService.DeactiveProductInput(id);

        if (resultDeactive.Success is false)
        {
            await DialogService.ShowMessageBox("", resultDeactive.Error, yesText: "Comprendo");
            return;
        }

        input.IsActive = !resultDeactive.Data;
    }
}