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
        private readonly ICartItemRepository _cartItemRepository;

        public CustomerService(ICustomerRepository customerRepository,
                                ITenantRepository tenantRepository,
                                ICartRepository cartRepository,
                                ICartItemRepository cartItemRepository)
        {
            _customerRepository = customerRepository;
            _tenantRepository = tenantRepository;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<ServiceResult<IEnumerable<CustomerDto>>> GetCustomersAsync(Guid tenantId, Guid ownerId, QueryCustomer query)
        {
            if (!await _tenantRepository.TenantExists(tenantId))
                return ServiceResult<IEnumerable<CustomerDto>>.Fail(404, "Tenant không tồn tại");
            if (!await _tenantRepository.IsTenantOwner(tenantId, ownerId))
                return ServiceResult<IEnumerable<CustomerDto>>.Forbidden("Bạn không có quyền truy cập vào tenant này");
            var customers = _customerRepository.GetCustomersByTenantQuery(tenantId);
            if (!string.IsNullOrEmpty(query.SearchTerm))
                customers = customers.Where(c => c.Email.ToLower().Contains(query.SearchTerm));
            if (!string.IsNullOrEmpty(query.Email))
                customers = customers.Where(c => c.Email.ToLower() == query.Email);
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
            if (!await _tenantRepository.TenantExists(tenantId))
                return ServiceResult<CustomerDto>.Fail(404, "Tenant không tồn tại");
            if (!await _tenantRepository.IsTenantOwner(tenantId, ownerId))
                return ServiceResult<CustomerDto>.Forbidden("Bạn không có quyền truy cập vào tenant này");
            var customer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (customer == null)
                return ServiceResult<CustomerDto>.Fail(404, "Customer không tồn tại");
            return ServiceResult<CustomerDto>.Ok(customer.ToCustomerDto());
        }
        public async Task<ServiceResult<object>> DeleteCustomerAsync(Guid tenantId, Guid customerId, Guid ownerId)
        {
            if (!await _tenantRepository.TenantExists(tenantId))
                return ServiceResult<object>.Fail(404, "Tenant không tồn tại");
            if (!await _tenantRepository.IsTenantOwner(tenantId, ownerId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền truy cập vào tenant này");
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<object>.Fail(404, "Customer không tồn tại");
            // Xóa cart item liên quan đến customer trước khi xóa customer
            foreach (var cartItem in await _cartItemRepository.GetCartItemsAsync(tenantId, customerId) ?? Enumerable.Empty<CartItem>())
                await _cartItemRepository.DeleteCartItemAsync(tenantId, customerId, cartItem.Id);
            await _cartRepository.DeleteCartAsync(tenantId, customerId);
            await _customerRepository.DeleteCustomerAsync(tenantId, customerId);
            return ServiceResult<object>.Ok(new { message = "Xóa customer thành công" });
        }
    }
}




