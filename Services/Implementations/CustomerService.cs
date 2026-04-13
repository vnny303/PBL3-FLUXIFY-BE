using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.IServices;

namespace FluxifyAPI.Services
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

        public async Task<ServiceResult<IEnumerable<CustomerDto>>> GetCustomersAsync(Guid tenantId, Guid ownerId)
        {
            var tenantResult = await IsTenantOwnerAsync(tenantId, ownerId);
            if (!tenantResult.Success)
                return ServiceResult<IEnumerable<CustomerDto>>.Fail(tenantResult.StatusCode, tenantResult.Message!);

            var customers = await _customerRepository.GetCustomersByTenantAsync(tenantId);
            return ServiceResult<IEnumerable<CustomerDto>>.Ok(customers.Select(c => c.ToCustomerDto()));
        }

        public async Task<ServiceResult<CustomerDto>> GetCustomerAsync(Guid tenantId, Guid customerId, Guid ownerId)
        {
            var tenantResult = await IsTenantOwnerAsync(tenantId, ownerId);
            if (!tenantResult.Success)
                return ServiceResult<CustomerDto>.Fail(tenantResult.StatusCode, tenantResult.Message!);

            var customer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (customer == null)
                return ServiceResult<CustomerDto>.Fail(404, "Customer không tồn tại");

            return ServiceResult<CustomerDto>.Ok(customer.ToCustomerDto());
        }

        public async Task<ServiceResult<CustomerDto>> GetCustomerByEmailAsync(Guid tenantId, string email, Guid ownerId)
        {
            var tenantResult = await IsTenantOwnerAsync(tenantId, ownerId);
            if (!tenantResult.Success)
                return ServiceResult<CustomerDto>.Fail(tenantResult.StatusCode, tenantResult.Message!);

            var customer = await _customerRepository.GetCustomerByEmailAsync(tenantId, email);
            if (customer == null)
                return ServiceResult<CustomerDto>.Fail(404, "Customer không tồn tại");

            return ServiceResult<CustomerDto>.Ok(customer.ToCustomerDto());
        }

        public async Task<ServiceResult<CustomerDto>> GetCustomerByCartAsync(Guid tenantId, Guid cartId, Guid ownerId)
        {
            var tenantResult = await IsTenantOwnerAsync(tenantId, ownerId);
            if (!tenantResult.Success)
                return ServiceResult<CustomerDto>.Fail(tenantResult.StatusCode, tenantResult.Message!);

            var customer = await _customerRepository.GetCustomerByCartAsync(tenantId, cartId);
            if (customer == null)
                return ServiceResult<CustomerDto>.Fail(404, "Customer không tồn tại");

            return ServiceResult<CustomerDto>.Ok(customer.ToCustomerDto());
        }

        public async Task<ServiceResult<CustomerDto>> CreateCustomerAsync(Guid tenantId, CreateCustomerRequestDto customerDto, Guid ownerId)
        {
            var tenantResult = await IsTenantOwnerAsync(tenantId, ownerId);
            if (!tenantResult.Success)
                return ServiceResult<CustomerDto>.Fail(tenantResult.StatusCode, tenantResult.Message!);

            var existingCustomer = await _customerRepository.GetCustomerByEmailAsync(tenantId, customerDto.Email);
            if (existingCustomer != null)
                return ServiceResult<CustomerDto>.Fail(400, "Email đã được đăng ký trong cửa hàng này");

            var customer = customerDto.ToCustomerFromCreateDto(tenantId);
            var createdCustomer = await _customerRepository.CreateCustomerAsync(customer);
            return ServiceResult<CustomerDto>.Created(createdCustomer.ToCustomerDto());
        }

        public async Task<ServiceResult<CustomerDto>> UpdateCustomerAsync(Guid tenantId, Guid customerId, UpdateCustomerRequestDto customerDto, Guid ownerId)
        {
            var tenantResult = await IsTenantOwnerAsync(tenantId, ownerId);
            if (!tenantResult.Success)
                return ServiceResult<CustomerDto>.Fail(tenantResult.StatusCode, tenantResult.Message!);

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
            var tenantResult = await IsTenantOwnerAsync(tenantId, ownerId);
            if (!tenantResult.Success)
                return ServiceResult<object>.Fail(tenantResult.StatusCode, tenantResult.Message!);

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

        private async Task<ServiceResult<object>> IsTenantOwnerAsync(Guid tenantId, Guid ownerId)
        {
            var tenant = await _tenantRepository.GetTenantAsync(tenantId);
            if (tenant == null)
                return ServiceResult<object>.Fail(404, "Tenant không tồn tại");

            if (tenant.OwnerId != ownerId)
                return ServiceResult<object>.Fail(403, "Bạn không có quyền truy cập tenant này");

            return ServiceResult<object>.Ok(new object());
        }
    }
}
