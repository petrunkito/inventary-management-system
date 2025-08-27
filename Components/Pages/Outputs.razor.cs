
using InventaryManagementSystem.Services;
using Microsoft.AspNetCore.Components;
using InventaryManagementSystem.Components.Dialog;
using MudBlazor;
using InventaryManagementSystem.Services.Result;
using InventaryManagementSystem.helpers.ProductOutputHelpers;

namespace InventaryManagementSystem.Components.Pages;

public partial class Outputs : ComponentBase
{

    [Inject]
    protected IProductOutputsService ProductOutputService { get; set; } = default!;
    [Inject]
    protected IDialogService DialogService { get; set; } = default!;

    private List<ProductOutputSelect> outputs = default!;

    protected override async Task OnInitializedAsync()
    {
        await FillOutputProducts();
    }

    private async Task FillOutputProducts()
    {
        outputs = await ProductOutputService.GetProductOutputs();
    }
    private async Task CreateOutputProduct()
    {
        string titleDialog = "Crear salida";
        var dialog = await DialogService.ShowAsync<CreateOutputDialog>(titleDialog);
        var result = await dialog.Result;

        if (result is null) return;
        if (result?.Canceled is true) return;

        ProductOutputCreate? detailOutput = (ProductOutputCreate?)result?.Data;
        if (detailOutput is null) return;

        Result<bool> resultCreation = await ProductOutputService.CreateProductOutput(detailOutput);
        if (resultCreation.Success is false)
        {
            await DialogService.ShowMessageBox("", resultCreation.Error ?? "Ocurrió un problema al tratar de crear la entrada para este producto.", yesText: "Comprendo");
            return;
        }
        await FillOutputProducts();
    }
    private async Task DeactiveOutputProduct(Guid id)
    {
        ProductOutputSelect? output = outputs.FirstOrDefault(x => x.ProductOutputId == id);
        if (output is null)
        {
            await DialogService.ShowMessageBox("", "No se encontró la salida.", yesText: "Comprendo");
            return;
        }        

        MarkupString message = (MarkupString)"¿Desea desactivar esta salida?<br/>Este cambio no es reversible";
        bool? result = await DialogService.ShowMessageBox("", message, yesText: "Sí", cancelText: "No");
        if (result is null) return;

        Result<bool> resultDeactive = await ProductOutputService.DeactiveProductOutput(id);

        if (resultDeactive.Success is false)
        {
            await DialogService.ShowMessageBox("", resultDeactive.Error, yesText: "Comprendo");
            return;
        }

        output.IsActive = !resultDeactive.Data;
    }
}