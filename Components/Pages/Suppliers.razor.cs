using InventaryManagementSystem.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using InventaryManagementSystem.Models.InventaryManagementSystem;
using InventaryManagementSystem.Components.Dialog;
using InventaryManagementSystem.helpers.SuppliersHelpers;
using InventaryManagementSystem.Services.Result;

namespace InventaryManagementSystem.Components.Pages
{
    public partial class Suppliers : ComponentBase
    {
        [Inject]
        protected ISuppliersService SuppliersService { get; set; } = default!;

        [Inject]
        protected IDialogService DialogService { get; set; } = default!;
        private List<Supplier> suppliers { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await FillSuppliers();
        }

        private async Task FillSuppliers()
        {
            suppliers = await SuppliersService.GetAllSuppliers();
        }
    
        private async Task CreateSupplier()
        {
            string titleDialog = "Crear proveedor";
            var dialog = await DialogService.ShowAsync<CreateEditSuppliersDialog>(titleDialog);
            var result = await dialog.Result;

            if (result is null) return;
            if (result?.Canceled is true) return;

            SupplierDetail? detailSupplier = (SupplierDetail?)result?.Data;
            if (detailSupplier is null) return;
            

            string? name = detailSupplier.Name?.Trim();
            string? numberPhone = detailSupplier.NumberPhone?.Trim();
            string? address = detailSupplier.Address?.Trim();


            if (string.IsNullOrEmpty(name))
            {
                await DialogService.ShowMessageBox("", "El nombre del proveedor no puede estar vacío.", yesText: "Comprendo");
                return;
            }

            Result<Supplier> resultCreation = await SuppliersService.CreateSupplier(new SupplierDetail
            {
                Name = name,
                NumberPhone = numberPhone,
                Address = address
            });

            if (resultCreation.Success is false)
            {
                await DialogService.ShowMessageBox("", resultCreation.Error ?? "Ocurrió un problema al tratar de crear al proveedor.", yesText: "Comprendo");
                return;
            }

            suppliers.Add(resultCreation.Data);
        }

        private async Task EditSupplier(Guid id)
        {
            string titleDialog = "Editar proveedor";

            Supplier? supplier = suppliers.FirstOrDefault(x => x.Id == id);
            if (supplier is null)
            {
                await DialogService.ShowMessageBox("", "No se encontró el proveedor.", yesText: "Comprendo");
                return;
            }

            //if the supplier is updated, then the supplier an the source is automatically updated, and we don't need to update manually at the end of code
            DialogParameters parameters = new DialogParameters<CreateEditSuppliersDialog>
            {
                {x=> x.SupplierDetail, new SupplierDetail{Name = supplier.Name, NumberPhone = supplier.NumberPhone, Address = supplier.Address}},
                {x=>x.ButtonOptions, ButtonsDialog.EditAndCancel}
            };
            var dialog = await DialogService.ShowAsync<CreateEditSuppliersDialog>(titleDialog, parameters);
            var result = await dialog.Result;

            if (result is null) return;
            if (result?.Canceled is true) return;

            SupplierDetail? detailSupplier = (SupplierDetail?)result!.Data;

            if (detailSupplier is null) return;

            string? name = detailSupplier.Name?.Trim();
            string? numberPhone = detailSupplier.NumberPhone?.Trim();
            string? address = detailSupplier.Address?.Trim();

            bool isTitleRepeated = suppliers.Any(x => x.Name == name && x.Id != id);// doesn't take into account the real supplier
            if (isTitleRepeated)
            {
                await DialogService.ShowMessageBox("", "El nombre del proveedor que deseas ingresar, ya está ocupado, ingresa uno diferente.", yesText: "Comprendo");
                return;
            }

            Result<Supplier> resultEdition = await SuppliersService.UpdateSupplier(id, new SupplierDetail
            {
                Name = name,
                NumberPhone = numberPhone,
                Address = address
            });

            if (resultEdition.Success is false)
            {
                await DialogService.ShowMessageBox("", resultEdition.Error, yesText: "Comprendo");
                return;
            }

            supplier.Name = resultEdition.Data.Name;
            supplier.Address = resultEdition.Data.Address;

        }

        private async Task DeactiveAndActivateSupplier(Guid id)
        {
            Supplier? supplier = suppliers.FirstOrDefault(x => x.Id == id);
            if (supplier is null)
            {
                await DialogService.ShowMessageBox("", "No se encontró el proveedor.", yesText: "Comprendo");
                return;
            }

            string message = supplier.IsActive ? "¿Desea desactivar este proveedor?" : "¿Desea activar este proveedor?";
            bool? result = await DialogService.ShowMessageBox("", message, yesText: "Sí", cancelText: "No");
            if (result is null) return;

            Result<bool> resultDeactiveActivate = await SuppliersService.DeactiveAndActivateSupplier(id);
            if (resultDeactiveActivate.Success is false)
            {
                await DialogService.ShowMessageBox("", resultDeactiveActivate.Error, yesText: "Comprendo");
                return;
            }

            supplier.IsActive = resultDeactiveActivate.Data;
        }
   

    }
    
}