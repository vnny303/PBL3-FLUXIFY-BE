using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Interfaces;
using FluxifyAPI.IServices;

namespace FluxifyAPI.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductSkuRepository _productSkuRepository;

        public CartItemService(
            ICartItemRepository cartItemRepository,
            ICustomerRepository customerRepository,
            IProductSkuRepository productSkuRepository)
        {
            _cartItemRepository = cartItemRepository;
            _customerRepository = customerRepository;
            _productSkuRepository = productSkuRepository;
        }

        public async Task<ServiceResult<IEnumerable<object>>> GetCartItemsAsync(Guid tenantId, Guid customerId)
        {
            var customer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (customer == null)
                return ServiceResult<IEnumerable<object>>.Fail(404, "Không tìm thấy khách hàng!");

            var items = await _cartItemRepository.GetCartItemsAsync(tenantId, customerId) ?? [];
            var result = items.Select(ci => new
            {
                id = ci.Id,
                productSkuId = ci.ProductSkuId,
                productId = ci.ProductSku.ProductId,
                productName = ci.ProductSku.Product.Name,
                productPrice = ci.ProductSku.Price,
                quantity = ci.Quantity,
                subTotal = ci.ProductSku.Price * ci.Quantity
            });

            return ServiceResult<IEnumerable<object>>.Ok(result);
        }

        public async Task<ServiceResult<object>> AddToCartAsync(Guid tenantId, Guid customerId, CreateCartItemRequestDto createDto)
        {
            var customer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (customer == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy khách hàng!");

            var sku = await _productSkuRepository.GetProductSkuAsync(tenantId, createDto.ProductSkuId);
            if (sku == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy SKU sản phẩm!");

            var items = await _cartItemRepository.GetCartItemsAsync(tenantId, customerId) ?? [];
            var existingItem = items.FirstOrDefault(i => i.ProductSkuId == createDto.ProductSkuId);
            var existingQuantity = existingItem?.Quantity ?? 0;

            if (sku.Stock < existingQuantity + createDto.Quantity)
                return ServiceResult<object>.Fail(400, $"SKU chỉ còn {sku.Stock} trong kho!");

            var addedItem = await _cartItemRepository.AddToCartAsync(tenantId, customerId, createDto.ProductSkuId, createDto.Quantity);
            var message = existingItem == null ? "Đã thêm vào giỏ hàng!" : "Đã cập nhật số lượng trong giỏ hàng!";

            return ServiceResult<object>.Ok(new
            {
                id = addedItem.Id,
                productSkuId = addedItem.ProductSkuId,
                quantity = addedItem.Quantity,
                message
            });
        }

        public async Task<ServiceResult<object>> UpdateCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId, UpdateCartItemRequestDto updateDto)
        {
            var customer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (customer == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy khách hàng!");

            var items = await _cartItemRepository.GetCartItemsAsync(tenantId, customerId) ?? [];
            var currentItem = items.FirstOrDefault(i => i.Id == cartItemId);
            if (currentItem == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy sản phẩm trong giỏ hàng!");

            var sku = await _productSkuRepository.GetProductSkuAsync(tenantId, currentItem.ProductSkuId);
            if (sku != null && sku.Stock < updateDto.Quantity)
                return ServiceResult<object>.Fail(400, $"SKU chỉ còn {sku.Stock} trong kho!");

            var updatedItem = await _cartItemRepository.UpdateCartItemAsync(tenantId, customerId, cartItemId, updateDto.Quantity);
            if (updatedItem == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy sản phẩm trong giỏ hàng!");

            return ServiceResult<object>.Ok(new
            {
                id = updatedItem.Id,
                productSkuId = updatedItem.ProductSkuId,
                quantity = updatedItem.Quantity,
                message = "Đã cập nhật giỏ hàng!"
            });
        }

        public async Task<ServiceResult<object>> RemoveCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId)
        {
            var customer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (customer == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy khách hàng!");

            var deletedItem = await _cartItemRepository.DeleteCartItemAsync(tenantId, customerId, cartItemId);
            if (deletedItem == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy sản phẩm trong giỏ hàng!");

            return ServiceResult<object>.Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng!" });
        }

        public async Task<ServiceResult<object>> ClearCartAsync(Guid tenantId, Guid customerId)
        {
            var customer = await _customerRepository.GetCustomerAsync(tenantId, customerId);
            if (customer == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy khách hàng!");

            var items = await _cartItemRepository.GetCartItemsAsync(tenantId, customerId) ?? [];
            foreach (var item in items)
            {
                await _cartItemRepository.DeleteCartItemAsync(tenantId, customerId, item.Id);
            }

            return ServiceResult<object>.Ok(new { message = $"Đã xóa {items.Count()} sản phẩm khỏi giỏ hàng!" });
        }
    }
}
