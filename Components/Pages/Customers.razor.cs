using InventaryManagementSystem.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using InventaryManagementSystem.Models.InventaryManagementSystem;
using InventaryManagementSystem.Components.Dialog;
using InventaryManagementSystem.helpers.SuppliersHelpers;
using InventaryManagementSystem.Services.Result;
using InventaryManagementSystem.helpers.CustomerHelpers;

namespace InventaryManagementSystem.Components.Pages
{
    public partial class Customers : ComponentBase
    {
        [Inject]
        protected ICustomerService CustomersService { get; set; } = default!;

        [Inject]
        protected IDialogService DialogService { get; set; } = default!;
        private List<Customer> customers { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await FillCustomers();
        }

        private async Task FillCustomers()
        {
            customers = await CustomersService.GetAllCustomers();
        }
    
        private async Task CreateCustomer()
        {
            string titleDialog = "Crear cliente";
            var dialog = await DialogService.ShowAsync<CreateEditCustomersDialog>(titleDialog);
            var result = await dialog.Result;

            if (result is null) return;
            if (result?.Canceled is true) return;

            CustomerCreateAndUpdateDetail? detailCustomer = (CustomerCreateAndUpdateDetail?)result?.Data;
            if (detailCustomer is null) return;

            string? name = detailCustomer.Name?.Trim();
            string? numberPhone = detailCustomer.NumberPhone?.Trim();
            string? address = detailCustomer.Address?.Trim();


            if (string.IsNullOrEmpty(name))
            {
                await DialogService.ShowMessageBox("", "El nombre del cliente no puede estar vacío.", yesText: "Comprendo");
                return;
            }

            Result<Customer> resultCreation = await CustomersService.CreateCustomer(new CustomerCreateAndUpdateDetail
            {
                Name = name,
                NumberPhone = numberPhone,
                Address = address
            });

            if (resultCreation.Success is false)
            {
                await DialogService.ShowMessageBox("", resultCreation.Error ?? "Ocurrió un problema al tratar de crear al cliente.", yesText: "Comprendo");
                return;
            }

            customers.Add(resultCreation.Data);
        }

        private async Task EditCustomer(Guid id)
        {
            string titleDialog = "Editar cliente";

            Customer? customer = customers.FirstOrDefault(x => x.Id == id);
            if (customer is null)
            {
                await DialogService.ShowMessageBox("", "No se encontró el cliente.", yesText: "Comprendo");
                return;
            }

            DialogParameters parameters = new DialogParameters<CreateEditCustomersDialog>
            {
                {x=> x.CustomerDetail, new CustomerCreateAndUpdateDetail{Name = customer.Name, NumberPhone = customer.NumberPhone, Address = customer.Address}},
                {x=>x.ButtonOptions, ButtonsDialog.EditAndCancel}
            };
            var dialog = await DialogService.ShowAsync<CreateEditCustomersDialog>(titleDialog, parameters);
            var result = await dialog.Result;

            if (result is null) return;
            if (result?.Canceled is true) return;

            CustomerCreateAndUpdateDetail? detailCustomer = (CustomerCreateAndUpdateDetail?)result!.Data;

            if (detailCustomer is null) return;

            string? name = detailCustomer.Name?.Trim();
            string? numberPhone = detailCustomer.NumberPhone?.Trim();
            string? address = detailCustomer.Address?.Trim();

            if (string.IsNullOrEmpty(name))
            {
                await DialogService.ShowMessageBox("", "El nombre del cliente no puede estar vacío.", yesText: "Comprendo");
                return;
            }

            Result<Customer> resultEdition = await CustomersService.UpdateCustomer(id, new CustomerCreateAndUpdateDetail
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

            customer.Name = resultEdition.Data.Name;
            customer.Address = resultEdition.Data.Address;

        }

        private async Task DeactiveAndActivateCustomer(Guid id)
        {
            Customer? customer = customers.FirstOrDefault(x => x.Id == id);
            if (customer is null)
            {
                await DialogService.ShowMessageBox("", "No se encontró el cliente.", yesText: "Comprendo");
                return;
            }

            string message = customer.IsActive ? "¿Desea desactivar este cliente?" : "¿Desea activar este cliente?";
            bool? result = await DialogService.ShowMessageBox("", message, yesText: "Sí", cancelText: "No");
            if (result is null) return;

            Result<bool> resultDeactiveActivate = await CustomersService.DeactiveAndActivateCustomer(id);
            if (resultDeactiveActivate.Success is false)
            {
                await DialogService.ShowMessageBox("", resultDeactiveActivate.Error, yesText: "Comprendo");
                return;
            }

            customer.IsActive = resultDeactiveActivate.Data;
        }
   

    }
    
}