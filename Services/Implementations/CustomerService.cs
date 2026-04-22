using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using FluxifyAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using FluxifyAPI.Models;

namespace FluxifyAPI.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICartRepository _cartRepository;

        public CustomerService(ICustomerRepository customerRepository, ITenantRepository tenantRepository, ICartRepository cartRepository)
        {
            _customerRepository = customerRepository;
            _tenantRepository = tenantRepository;
            _cartRepository = cartRepository;
        }

        public async Task<ServiceResult<IEnumerable<CustomerDto>>> GetCustomersAsync(Guid tenantId, Guid ownerId, QueryCustomer query)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, ownerId))
                return ServiceResult<IEnumerable<CustomerDto>>.Forbidden("Bạn không có quyền truy cập vào tenant này");

            var customers = _customerRepository.GetCustomer(tenantId);

            if (!string.IsNullOrEmpty(query.SearchTerm))
                customers = customers.Where(c => c.Email.ToLower().Contains(query.SearchTerm));
            if (!string.IsNullOrEmpty(query.Email))
                customers = customers.Where(c => c.Email.ToLower() == query.Email.Trim().ToLower());
            if (query.IsActive.HasValue)
                customers = customers.Where(c => c.IsActive == query.IsActive.Value);
            if (query.CreatedFrom.HasValue)
                customers = customers.Where(c => c.CreatedAt >= query.CreatedFrom.Value);
            if (query.CreatedTo.HasValue)
                customers = customers.Where(c => c.CreatedAt <= query.CreatedTo.Value);

            var isDescending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            switch (query.SortBy)
            {
                case "email":
                    customers = isDescending ? customers.OrderByDescending(c => c.Email) : customers.OrderBy(c => c.Email);
                    break;
                case "createdat":
                case "created_at":
                    customers = isDescending ? customers.OrderByDescending(c => c.CreatedAt) : customers.OrderBy(c => c.CreatedAt);
                    break;
                default:
                    customers = customers.OrderByDescending(c => c.CreatedAt);
                    break;
            }

            var skipNumber = (query.Page - 1) * query.PageSize;
            var pagedCustomers = await customers.Skip(skipNumber).Take(query.PageSize).ToListAsync();

            return ServiceResult<IEnumerable<CustomerDto>>.Ok(pagedCustomers.Select(c => c.ToCustomerDto()));
        }

        public async Task<ServiceResult<CustomerDto>> GetCustomerAsync(Guid tenantId, Guid customerId, Guid ownerId)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, ownerId))
                return ServiceResult<CustomerDto>.Forbidden("Bạn không có quyền truy cập vào tenant này");

            var customer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (customer == null)
                return ServiceResult<CustomerDto>.Fail(404, "Customer không tồn tại");

            return ServiceResult<CustomerDto>.Ok(customer.ToCustomerDto());
        }

        public async Task<ServiceResult<CustomerDto>> GetCustomerByEmailAsync(Guid tenantId, string email, Guid ownerId)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, ownerId))
                return ServiceResult<CustomerDto>.Forbidden("Bạn không có quyền truy cập vào tenant này");
            var customer = await _customerRepository.GetCustomerByEmailAsync(tenantId, email);
            if (customer == null)
                return ServiceResult<CustomerDto>.Fail(404, "Customer không tồn tại");

            return ServiceResult<CustomerDto>.Ok(customer.ToCustomerDto());
        }

        // public async Task<ServiceResult<CustomerDto>> GetCustomerByCartAsync(Guid tenantId, Guid cartId, Guid ownerId)
        // {
        //     var tenantResult = await _tenantRepository.IsTenantOwner(tenantId, ownerId);
        //     if (!await _tenantRepository.IsTenantOwner(tenantId, ownerId))
        //         return ServiceResult<CustomerDto>.Forbidden("Bạn không có quyền truy cập vào tenant này");

        //     var customer = await _customerRepository.GetCustomerByCartAsync(tenantId, cartId);
        //     if (customer == null)
        //         return ServiceResult<CustomerDto>.Fail(404, "Customer không tồn tại");

        //     return ServiceResult<CustomerDto>.Ok(customer.ToCustomerDto());
        // }

        public async Task<ServiceResult<CustomerDto>> CreateCustomerAsync(Guid tenantId, CreateCustomerRequestDto customerDto, Guid ownerId)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, ownerId))
                return ServiceResult<CustomerDto>.Forbidden("Bạn không có quyền truy cập vào tenant này");

            var existingCustomer = await _customerRepository.GetCustomerByEmailAsync(tenantId, customerDto.Email);
            if (existingCustomer != null)
                return ServiceResult<CustomerDto>.Fail(400, "Email đã được đăng ký trong cửa hàng này");

            var customer = customerDto.ToCustomerFromCreateDto(tenantId);
            var createdCustomer = await _customerRepository.CreateCustomerAsync(customer);
            var cart = await _cartRepository.CreateCartAsync(new Cart
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerId = createdCustomer.Id
            });

            createdCustomer.Cart = cart;
            return ServiceResult<CustomerDto>.Created(createdCustomer.ToCustomerDto());
        }

        public async Task<ServiceResult<CustomerDto>> UpdateCustomerAsync(Guid tenantId, Guid customerId, UpdateCustomerRequestDto customerDto, Guid ownerId)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, ownerId))
                return ServiceResult<CustomerDto>.Forbidden("Bạn không có quyền truy cập vào tenant này");

            var existingCustomer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (existingCustomer == null)
                return ServiceResult<CustomerDto>.Fail(404, "Customer không tồn tại");

            if (!string.IsNullOrWhiteSpace(customerDto.Email))
            {
                var normalizedEmail = customerDto.Email.Trim();
                if (!string.Equals(normalizedEmail, existingCustomer.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailInUse = await _customerRepository.GetCustomerByEmailAsync(tenantId, normalizedEmail);
                    if (emailInUse != null && emailInUse.Id != customerId)
                        return ServiceResult<CustomerDto>.Fail(400, "Email đã được đăng ký trong cửa hàng này");
                }
            }

            customerDto.ToCustomerFromUpdateDto(existingCustomer);
            var updatedCustomer = await _customerRepository.UpdateCustomerAsync(existingCustomer);

            return ServiceResult<CustomerDto>.Ok(updatedCustomer.ToCustomerDto());
        }

        public async Task<ServiceResult<object>> DeleteCustomerAsync(Guid tenantId, Guid customerId, Guid ownerId)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, ownerId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền truy cập vào tenant này");

            var existingCustomer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (existingCustomer == null)
                return ServiceResult<object>.Fail(404, "Customer không tồn tại");

            await _customerRepository.DeleteCustomerAsync(tenantId, customerId);
            return new ServiceResult<object>
            {
                Success = true,
                StatusCode = 204,
                Data = null
            };
        }
    }
}




