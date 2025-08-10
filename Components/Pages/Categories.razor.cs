using InventaryManagementSystem.Components.Dialog;
using InventaryManagementSystem.Services;
using InventaryManagementSystem.Models.InventaryManagementSystem;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using InventaryManagementSystem.helpers.CategoriesHelpers;
using InventaryManagementSystem.Services.Result;



namespace InventaryManagementSystem.Components.Pages
{
    public partial class Categories : ComponentBase
    {
        [Inject]
        protected ICategoriesService CategoriesService { get; set; } = default!;

        [Inject]
        protected IDialogService DialogService { get; set; } = default!;

        private List<Category> categories { get; set; } = default!;


        protected override async Task OnInitializedAsync()
        {

            await FillCategories();
        }

        private async Task FillCategories()
        {
            categories = await CategoriesService.GetAllCategories();
        }

        private async Task CreateCategory()
        {
            string titleDialog = "Crear categoría";
            var dialog = await DialogService.ShowAsync<CreateEditCategoriesDialog>(titleDialog);
            var result = await dialog.Result;

            if (result is null) return;
            if (result?.Canceled is true) return;

            CategoryDetail? detailCategorie = (CategoryDetail?)result?.Data;
            if (detailCategorie is null) return;

            string? title = detailCategorie.Title?.Trim();
            string? description = detailCategorie.Description?.Trim();

            //hay que hacer la validacion que no hacemos con los dto pero en el frontend
            if (string.IsNullOrEmpty(title))
            {
                await DialogService.ShowMessageBox("", "El título de la categoría no puede estar vacío.", yesText: "Comprendo");
                return;
            }

            Result<Category> resultCreation = await CategoriesService.CreateCategory(new CategoryDetail
            {
                Title = title,
                Description = description
            });

            if (resultCreation.Success is false)
            {
                await DialogService.ShowMessageBox("", resultCreation.Error ?? "Ocurrió un problema al tratar de crear la categoría.", yesText: "Comprendo");
                return;
            }

            categories.Add(resultCreation.Data);
        }

        private async Task EditCategory(Guid id)
        {
            string titleDialog = "Editar categoría";

            Category? category = categories.FirstOrDefault(x => x.Id == id);
            if (category is null)
            {
                await DialogService.ShowMessageBox("", "No se encontró la categoría.", yesText: "Comprendo");
                return;
            }

            //if the category is updated, then the category an the source is automatically updated, and we don't need to update manually at the end of code
            DialogParameters parameters = new DialogParameters<CreateEditCategoriesDialog>
            {
                {x=> x.CategoryDetail, new CategoryDetail{Title = category.Title, Description = category.Description}},
                {x=>x.ButtonOptions, ButtonsDialog.EditAndCancel}
            };
            var dialog = await DialogService.ShowAsync<CreateEditCategoriesDialog>(titleDialog, parameters);
            var result = await dialog.Result;

            if (result is null) return;
            if (result?.Canceled is true) return;

            CategoryDetail? detailCategorie = (CategoryDetail?)result!.Data;

            if (detailCategorie is null) return;

            string? title = detailCategorie.Title?.Trim();
            string? description = detailCategorie.Description?.Trim();

            bool isTitleRepeated = categories.Any(x => x.Title == title && x.Id != id);// doesn't take into account the real category
            if (isTitleRepeated)
            {
                await DialogService.ShowMessageBox("", "El título de la categoría que deseas ingresar, ya está ocupado, ingresa uno diferente.", yesText: "Comprendo");
                return;
            }

            Result<Category> resultEdition = await CategoriesService.UpdateCategory(id, new CategoryDetail
            {
                Title = title,
                Description = description
            });

            if (resultEdition.Success is false)
            {
                await DialogService.ShowMessageBox("", resultEdition.Error, yesText: "Comprendo");
                return;
            }

            category.Title = resultEdition.Data.Title;
            category.Description = resultEdition.Data.Description;

        }

        private async Task DeactiveAndActivateCategory(Guid id)
        {
            Category? category = categories.FirstOrDefault(x => x.Id == id);
            if (category is null)
            {
                await DialogService.ShowMessageBox("", "No se encontró la categoría.", yesText: "Comprendo");
                return;
            }

            string message = category.IsActive ? "¿Desea desactivar esta categoría?" : "¿Desea activar esta categoría?";
            bool? result = await DialogService.ShowMessageBox("", message, yesText: "Sí", cancelText: "No");
            if (result is null) return;

            Result<bool> resultDeactiveActivate = await CategoriesService.DeactiveAndActivateCategory(id);
            if (resultDeactiveActivate.Success is false)
            {
                await DialogService.ShowMessageBox("", resultDeactiveActivate.Error, yesText: "Comprendo");
                return;
            }

            category.IsActive = resultDeactiveActivate.Data;



        }
    }
}
