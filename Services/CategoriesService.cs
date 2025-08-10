using InventaryManagementSystem.Models.InventaryManagementSystem;
using Microsoft.EntityFrameworkCore;
using InventaryManagementSystem.Services.Result;
using InventaryManagementSystem.helpers.CategoriesHelpers;

namespace InventaryManagementSystem.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ApplicationDbContext context;
        public CategoriesService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            List<Category> categories = await (from c in context.Categories orderby c.CreatedAt ascending select c).ToListAsync();
            return categories;
        }

        public async Task<Result<Category>> CreateCategory(CategoryDetail createCategoryDetail)
        {
            if (string.IsNullOrEmpty(createCategoryDetail.Title?.Trim()))
                return Result<Category>.Failure("El título de la categoría no puede estar vacío");

            bool existCategory = context.Categories.Any(category => category.Title == createCategoryDetail.Title);
            if (existCategory) return Result<Category>.Failure("El título ya existe, elige otro");

            Category newCategory = new()
            {
                Title = createCategoryDetail.Title,
                Description = createCategoryDetail.Description,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            context.Categories.Add(newCategory);
            await context.SaveChangesAsync();

            return Result<Category>.Ok(newCategory);
        }

        public async Task<Result<bool>> DeactiveAndActivateCategory(Guid uuid)
        {
            Category? category = context.Categories.FirstOrDefault(category => category.Id == uuid);
            if (category is null) return Result<bool>.Failure("No se encontro la categoría especificada");

            category.IsActive = !category.IsActive;
            await context.SaveChangesAsync();
            return Result<bool>.Ok(category.IsActive);
        }

        public async Task<Result<Category>> UpdateCategory(Guid uuid, CategoryDetail updateCategoryDetail)
        {
            if (string.IsNullOrEmpty(updateCategoryDetail.Title)  &&
            string.IsNullOrEmpty(updateCategoryDetail.Description))
                return Result<Category>.Failure("Debes proporcionar algún dato para actualizar la categoría.");

            string? title = updateCategoryDetail.Title?.Trim();
            string? description = updateCategoryDetail.Description?.Trim();
            
            List<Category> categories = (from c in context.Categories where c.Id == uuid || c.Title == title select c).ToList();
            if(categories.Count > 1)
                return Result<Category>.Failure($"Este título '{title}' ya se encuentra ocupado.");

            Category? category = categories.FirstOrDefault(x=>x.Id == uuid);
            if (category is null)
                return Result<Category>.Failure("No se encontro la categoría.");


            if (title is not null && title.Length > 0) category.Title = title;
            if (description is not null && description.Length > 0) category.Description = description;


            await context.SaveChangesAsync();
            return Result<Category>.Ok(category);
        }
    }

    public interface ICategoriesService
    {
        Task<List<Category>> GetAllCategories();
        Task<Result<Category>> CreateCategory(CategoryDetail createCategory);
        Task<Result<bool>> DeactiveAndActivateCategory(Guid uuid);
        Task<Result<Category>> UpdateCategory(Guid uuid, CategoryDetail updateCategory);
    }
}
