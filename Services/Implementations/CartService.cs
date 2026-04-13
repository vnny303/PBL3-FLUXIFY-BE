using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Implementations {
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITenantRepository _tenantRepository;

        public CartService(
            ICartRepository cartRepository,
            ICustomerRepository customerRepository,
            ITenantRepository tenantRepository)
        {
            _cartRepository = cartRepository;
            _customerRepository = customerRepository;
            _tenantRepository = tenantRepository;
        }

        public async Task<ServiceResult<IEnumerable<CartDto>>> GetCartsAsync(Guid tenantId, Guid customerId)
        {
            var cart = await _cartRepository.GetCartAsync(tenantId, customerId);
            if (cart == null)
                return ServiceResult<IEnumerable<CartDto>>.Ok(Array.Empty<CartDto>());

            return ServiceResult<IEnumerable<CartDto>>.Ok([cart.ToCartDto()]);
        }

        public async Task<ServiceResult<CartDto>> CreateCartAsync(Guid tenantId, Guid customerId, CreateCartRequestDto createDto)
        {
            if (createDto.TenantId.HasValue && createDto.TenantId.Value != tenantId)
                return ServiceResult<CartDto>.Fail(400, "TenantId trong body không khớp route");

            if (createDto.CustomerId.HasValue && createDto.CustomerId.Value != customerId)
                return ServiceResult<CartDto>.Fail(400, "CustomerId trong body không khớp route");

            if (await _customerRepository.GetCustomerAsync(tenantId, customerId) == null)
                return ServiceResult<CartDto>.Fail(404, "Customer không tồn tại");

            if (await _tenantRepository.GetTenantAsync(tenantId) == null)
                return ServiceResult<CartDto>.Fail(404, "Tenant không tồn tại");

            if (await _cartRepository.GetCartAsync(tenantId, customerId) != null)
                return ServiceResult<CartDto>.Fail(400, "Customer đã có giỏ hàng");

            var createdCart = await _cartRepository.CreateCartAsync(tenantId, customerId);
            return ServiceResult<CartDto>.Created(createdCart.ToCartDto());
        }
    }
}




