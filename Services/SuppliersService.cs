
using InventaryManagementSystem.Models.InventaryManagementSystem;
using InventaryManagementSystem.Services.Result;
using Microsoft.EntityFrameworkCore;
using InventaryManagementSystem.helpers.SuppliersHelpers;
using System.Text.RegularExpressions;

namespace InventaryManagementSystem.Services
{
    public class SuppliersService : ISuppliersService
    {
        private readonly ApplicationDbContext context;
        public SuppliersService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<List<Supplier>> GetAllSuppliers()
        {
            List<Supplier> suppliers = await (from s in context.Suppliers orderby s.CreatedAt ascending select s).ToListAsync();
            return suppliers;
        }

        public async Task<Result<Supplier>> CreateSupplier(SupplierDetail createSupplierDetail)
        {
            string? name = createSupplierDetail.Name?.Trim();
            if (string.IsNullOrEmpty(name))
                return Result<Supplier>.Failure("El nombre del proveedor no puede estar vacío");

            bool existSupplier = await context.Suppliers.AnyAsync(supplier => supplier.Name == name);
            if (existSupplier)
                return Result<Supplier>.Failure("Este proveedor ya existe, elige otro nombre.");

            string? numberPhone = createSupplierDetail.NumberPhone?.Trim();

            if (numberPhone is not null && numberPhone.Length > 0)
            {
                Regex regex = new Regex(@"^\d{4}-\d{4}$");
                if (!regex.IsMatch(numberPhone))
                    return Result<Supplier>.Failure("El formato del número de teléfono no es válido 'xxxx-xxxx'.");
            }
            

            Supplier newSupplier = new()
            {
                Name = name,
                Address = createSupplierDetail.Address,
                NumberPhone = createSupplierDetail.NumberPhone,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            context.Suppliers.Add(newSupplier);
            await context.SaveChangesAsync();
            return Result<Supplier>.Ok(newSupplier);

        }

        public async Task<Result<bool>> DeactiveAndActivateSupplier(Guid uuid)
        {
            
            Supplier? supplier = await context.Suppliers.FirstOrDefaultAsync(supplier => supplier.Id == uuid);
            if (supplier is null) return Result<bool>.Failure("No se encontro el proveedor.");

            supplier.IsActive = !supplier.IsActive;
            await context.SaveChangesAsync();
            return Result<bool>.Ok(supplier.IsActive);
        }

        public async Task<Result<Supplier>> UpdateSupplier(Guid uuid, SupplierDetail updateSupplierDetail)
        {
            if (string.IsNullOrEmpty(updateSupplierDetail.Name) &&
                string.IsNullOrEmpty(updateSupplierDetail.Address) &&
                string.IsNullOrEmpty(updateSupplierDetail.NumberPhone))
                return Result<Supplier>.Failure("Debes proporcionar algún dato para actualizar al proveedor.");

            string? name = updateSupplierDetail.Name?.Trim();
            string? address = updateSupplierDetail.Address?.Trim();
            string? numberPhone = updateSupplierDetail.NumberPhone?.Trim();
            
            List<Supplier> suppliers = await (from s in context.Suppliers where s.Id == uuid || s.Name == name select s).ToListAsync();
            if(suppliers.Count > 1)
                return Result<Supplier>.Failure($"Este nombre '{name}' ya se encuentra ocupado.");

            Supplier? supplier = suppliers.FirstOrDefault(x=>x.Id == uuid);
            if (supplier is null)
                return Result<Supplier>.Failure("No se encontro el proveedor.");

            if (name is not null && name.Length > 0) supplier.Name = name;
            if (address is not null && address.Length > 0) supplier.Address = address;
            if (numberPhone is not null && numberPhone.Length > 0)
            {
                Regex regex = new Regex(@"^\d{4}-\d{4}$");
                if (!regex.IsMatch(numberPhone))
                    return Result<Supplier>.Failure("El formato del número de teléfono no es válido 'xxxx-xxxx'.");

                supplier.NumberPhone = numberPhone;
            }

            await context.SaveChangesAsync();
            return Result<Supplier>.Ok(supplier);
        }
    }
    public interface ISuppliersService
    {
        Task<List<Supplier>> GetAllSuppliers();
        Task<Result<Supplier>> CreateSupplier(SupplierDetail createSupplierDetail);
        Task<Result<bool>> DeactiveAndActivateSupplier(Guid uuid);
        Task<Result<Supplier>> UpdateSupplier(Guid uuid, SupplierDetail updateSupplier);
    }
}
