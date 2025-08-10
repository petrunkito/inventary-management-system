
using InventaryManagementSystem.Models.InventaryManagementSystem;
using InventaryManagementSystem.Services.Result;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using InventaryManagementSystem.helpers.CustomerHelpers;

namespace InventaryManagementSystem.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext context;
        public CustomerService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<List<Customer>> GetAllCustomers()
        {
            List<Customer> customers = await (from s in context.Customers orderby s.CreatedAt ascending select s).ToListAsync();
            return customers;
        }

        public async Task<Result<Customer>> CreateCustomer(CustomerCreateAndUpdateDetail createCustomerDetail)
        {
            string? name = createCustomerDetail.Name?.Trim();
            if (string.IsNullOrEmpty(name))
                return Result<Customer>.Failure("El nombre del cliente no puede estar vacío");

            string? numberPhone = createCustomerDetail.NumberPhone?.Trim();

            if (numberPhone is not null && numberPhone.Length > 0)
            {
                Regex regex = new Regex(@"^\d{4}-\d{4}$");
                if (!regex.IsMatch(numberPhone))
                    return Result<Customer>.Failure("El formato del número de teléfono no es válido 'xxxx-xxxx'.");
            }            

            Customer newCustomer = new()
            {
                Name = name,
                Address = createCustomerDetail.Address,
                NumberPhone = createCustomerDetail.NumberPhone,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            context.Customers.Add(newCustomer);
            await context.SaveChangesAsync();
            return Result<Customer>.Ok(newCustomer);

        }

        public async Task<Result<bool>> DeactiveAndActivateCustomer(Guid uuid)
        {

            Customer? customer = await context.Customers.FirstOrDefaultAsync(customer => customer.Id == uuid);
            if (customer is null) return Result<bool>.Failure("No se encontro el cliente.");

            customer.IsActive = !customer.IsActive;
            await context.SaveChangesAsync();
            return Result<bool>.Ok(customer.IsActive);
        }

        public async Task<Result<Customer>> UpdateCustomer(Guid uuid, CustomerCreateAndUpdateDetail updateCustomerDetail)
        {
            if (string.IsNullOrEmpty(updateCustomerDetail.Name) &&
                string.IsNullOrEmpty(updateCustomerDetail.Address) &&
                string.IsNullOrEmpty(updateCustomerDetail.NumberPhone))
                return Result<Customer>.Failure("Debes proporcionar algún dato para actualizar al cliente.");

            string? name = updateCustomerDetail.Name?.Trim();
            string? address = updateCustomerDetail.Address?.Trim();
            string? numberPhone = updateCustomerDetail.NumberPhone?.Trim();

            Customer? customer = await (from s in context.Customers where s.Id == uuid select s).FirstOrDefaultAsync();
            if (customer is null)
                return Result<Customer>.Failure("No se encontro el cliente.");

            if (name is not null && name.Length > 0) customer.Name = name;
            if (address is not null && address.Length > 0) customer.Address = address;
            if (numberPhone is not null && numberPhone.Length > 0)
            {
                Regex regex = new Regex(@"^\d{4}-\d{4}$");
                if (!regex.IsMatch(numberPhone))
                    return Result<Customer>.Failure("El formato del número de teléfono no es válido 'xxxx-xxxx'.");

                customer.NumberPhone = numberPhone;
            }

            await context.SaveChangesAsync();
            return Result<Customer>.Ok(customer);
        }
    }
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomers();
        Task<Result<Customer>> CreateCustomer(CustomerCreateAndUpdateDetail createCustomerDetail);
        Task<Result<bool>> DeactiveAndActivateCustomer(Guid uuid);
        Task<Result<Customer>> UpdateCustomer(Guid uuid, CustomerCreateAndUpdateDetail updateCustomer);
    }
}
