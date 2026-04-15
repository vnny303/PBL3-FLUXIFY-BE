using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using FluxifyAPI.Mapper;

namespace FluxifyAPI.Services.Implementations
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductSkuRepository _productSkuRepository;

        public CartItemService(
            ICartItemRepository cartItemRepository,
            ICartRepository cartRepository,
            ICustomerRepository customerRepository,
            IProductSkuRepository productSkuRepository)
        {
            _cartItemRepository = cartItemRepository;
            _cartRepository = cartRepository;
            _customerRepository = customerRepository;
            _productSkuRepository = productSkuRepository;
        }

        public async Task<ServiceResult<IEnumerable<CartItemDto?>>> GetCartItemsAsync(Guid tenantId, Guid customerId)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<IEnumerable<CartItemDto?>>.Fail(404, "Không tìm thấy khách hàng!");
            var items = await _cartItemRepository.GetCartItemsAsync(tenantId, customerId);
            if (items == null)
                return ServiceResult<IEnumerable<CartItemDto?>>.Ok([]);
            return ServiceResult<IEnumerable<CartItemDto?>>.Ok(items.Select(i => i.ToCartItemDto()));
        }

        public async Task<ServiceResult<CartItemDto>> AddToCartAsync(Guid tenantId, Guid customerId, CreateCartItemRequestDto createDto)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<CartItemDto>.Fail(404, "Không tìm thấy khách hàng!");
            if (!await _productSkuRepository.ProductSkuExists(tenantId, createDto.ProductSkuId))
                return ServiceResult<CartItemDto>.Fail(404, "Không tìm thấy SKU sản phẩm!");
            var sku = await _productSkuRepository.GetProductSkusAsync(tenantId, createDto.ProductSkuId);
            if (!await _cartRepository.CartExists(tenantId, customerId))
                return ServiceResult<CartItemDto>.Fail(404, "Không tìm thấy giỏ hàng!");
            var cart = await _cartRepository.GetCartAsync(tenantId, customerId);
            if (!await _cartRepository.CartContainsProductSku(tenantId, customerId, createDto.ProductSkuId))
            {
                if (sku.Stock < createDto.Quantity)
                    return ServiceResult<CartItemDto>.Fail(400, $"SKU chỉ còn {sku.Stock} trong kho!");
                var cartItem = createDto.ToCartItemFromCreateDto(cart.Id);
                var addedItem = await _cartItemRepository.AddToCartAsync(cartItem);
                if (addedItem == null)
                    return ServiceResult<CartItemDto>.Fail(500, "Không thể thêm sản phẩm vào giỏ hàng!");
                return ServiceResult<CartItemDto>.Ok(addedItem.ToCartItemDto());
            }
            else
            {
                var cartItem = await _cartItemRepository.GetCartItemAsync(tenantId, customerId, createDto.ProductSkuId);
                if (cartItem == null)
                    return ServiceResult<CartItemDto>.Fail(404, "Không tìm thấy sản phẩm trong giỏ hàng!");
                if (sku.Stock < cartItem.Quantity + createDto.Quantity)
                    return ServiceResult<CartItemDto>.Fail(400, $"SKU chỉ còn {sku.Stock} trong kho!");
                cartItem.Quantity += createDto.Quantity;
                await _cartItemRepository.UpdateCartItemAsync(cartItem);
                return ServiceResult<CartItemDto>.Ok(cartItem.ToCartItemDto());
            }
        }

        public async Task<ServiceResult<CartItemDto>> UpdateCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId, UpdateCartItemRequestDto updateDto)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<CartItemDto>.Fail(404, "Không tìm thấy khách hàng!");
            if (!await _productSkuRepository.ProductSkuExists(tenantId, updateDto.ProductSkuId))
                return ServiceResult<CartItemDto>.Fail(404, "Không tìm thấy SKU sản phẩm!");
            var sku = await _productSkuRepository.GetProductSkusAsync(tenantId, updateDto.ProductSkuId);
            if (!await _cartRepository.CartExists(tenantId, customerId))
                return ServiceResult<CartItemDto>.Fail(404, "Không tìm thấy giỏ hàng!");
            var cart = await _cartRepository.GetCartAsync(tenantId, customerId);
            if (sku.Stock < updateDto.Quantity)
                return ServiceResult<CartItemDto>.Fail(400, $"SKU chỉ còn {sku.Stock} trong kho!");
            var cartItem = updateDto.ToCartItemFromUpdateDto(cartItemId);
            if (cartItem == null)
                return ServiceResult<CartItemDto>.Fail(404, "Không tìm thấy sản phẩm trong giỏ hàng!");

            return ServiceResult<CartItemDto>.Ok(cartItem.ToCartItemDto());
        }

        public async Task<ServiceResult<object>> RemoveCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<object>.Fail(404, "Không tìm thấy khách hàng!");
            var deletedItem = await _cartItemRepository.DeleteCartItemAsync(tenantId, customerId, cartItemId);
            if (deletedItem == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy sản phẩm trong giỏ hàng!");
            return ServiceResult<object>.Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng!" });
        }

        public async Task<ServiceResult<object>> ClearCartAsync(Guid tenantId, Guid customerId)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<object>.Fail(404, "Không tìm thấy khách hàng!");
            var items = await _cartItemRepository.GetCartItemsAsync(tenantId, customerId);
            if (items == null || !items.Any())
                return ServiceResult<object>.Ok(new { message = "Giỏ hàng đã trống!" });
            foreach (var item in items)
                await _cartItemRepository.DeleteCartItemAsync(tenantId, customerId, item.Id);
            return ServiceResult<object>.Ok(new { message = $"Đã xóa {items.Count()} sản phẩm khỏi giỏ hàng!" });
        }
    }
}