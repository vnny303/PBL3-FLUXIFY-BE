using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using FluxifyAPI.Helpers;

namespace FluxifyAPI.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ITenantRepository _tenantRepository;

        public CustomerService(ICustomerRepository customerRepository, ITenantRepository tenantRepository)
        {
            _customerRepository = customerRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<ServiceResult<IEnumerable<CustomerDto>>> GetCustomersAsync(Guid tenantId, Guid ownerId, QueryCustomer query)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, ownerId))
                return ServiceResult<IEnumerable<CustomerDto>>.Forbidden("Bạn không có quyền truy cập vào tenant này");
            var customers = _customerRepository.GetCustomer(tenantId);
            if (!string.IsNullOrEmpty(query.SearchTerm))
                customers = customers.Where(c => c.Email.ToLower().Contains(query.SearchTerm.Trim().ToLower()));
            if (!string.IsNullOrEmpty(query.Email))
                customers = customers.Where(c => c.Email.ToLower() == query.Email.Trim().ToLower());
            if (query.CreatedFrom.HasValue)
                customers = customers.Where(c => c.CreatedAt >= query.CreatedFrom.Value);
            if (query.CreatedTo.HasValue)
                customers = customers.Where(c => c.CreatedAt <= query.CreatedTo.Value);
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                switch (query.SortBy.ToLower())
                {
                    case "email":
                        customers = query.SortDirection == "desc" ? customers.OrderByDescending(c => c.Email) : customers.OrderBy(c => c.Email);
                        break;
                    case "createdat":
                        customers = query.SortDirection == "desc" ? customers.OrderByDescending(c => c.CreatedAt) : customers.OrderBy(c => c.CreatedAt);
                        break;
                    default:
                        customers = customers.OrderBy(c => c.Email); // Mặc định sắp xếp theo email
                        break;
                }
            }

            return ServiceResult<IEnumerable<CustomerDto>>.Ok(customers.Select(c => c.ToCustomerDto()));
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




