using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Implementations
{
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

        public async Task<ServiceResult<CartDto>> GetCartsAsync(Guid tenantId, Guid customerId)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<CartDto>.Fail(404, "Customer không tồn tại");
            if (!await _cartRepository.CartExists(tenantId, customerId))
                return ServiceResult<CartDto>.Fail(404, "Giỏ hàng không tồn tại");
            var cart = await _cartRepository.GetCartAsync(tenantId, customerId);
            if (cart == null)
                return ServiceResult<CartDto>.Fail(404, "Giỏ hàng không tồn tại");
            return ServiceResult<CartDto>.Ok(cart.ToCartDto());
        }

        public async Task<ServiceResult<CartDto>> CreateCartAsync(CreateCartRequestDto createDto)
        {
            if (!await _tenantRepository.TenantExists(createDto.TenantId))
                return ServiceResult<CartDto>.Fail(404, "Tenant không tồn tại");
            if (!await _customerRepository.CustomerExists(createDto.TenantId, createDto.CustomerId))
                return ServiceResult<CartDto>.Fail(404, "Customer không tồn tại");
            if (await _cartRepository.CartExists(createDto.TenantId, createDto.CustomerId))
                return ServiceResult<CartDto>.Fail(400, "Customer đã có giỏ hàng");

            var cart = createDto.ToCartFromCreateDto();
            var createdCart = await _cartRepository.CreateCartAsync(cart);
            return ServiceResult<CartDto>.Created(createdCart.ToCartDto());
        }
    }
}