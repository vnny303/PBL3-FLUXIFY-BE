using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluxifyAPI.Data;
using FluxifyAPI.Models;
using System.Text.Json;

namespace FluxifyAPI.Controllers
{
    [Route("api/tenants/{tenantId}/customers/{customerId}/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Lấy toàn bộ giỏ hàng của customer
        [HttpGet]
        public async Task<ActionResult> GetCartItems(Guid tenantId, Guid customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId && c.TenantId == tenantId);

            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng!" });

            var items = await _context.CartItems
                .Where(ci => ci.Cart.CustomerId == customerId)
                .Include(ci => ci.ProductSku)
                    .ThenInclude(s => s.Product)
                .ToListAsync();

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

            return Ok(result);
        }

        // POST: Thêm sản phẩm vào giỏ hàng (nếu đã có thì cộng dồn số lượng)
        [HttpPost]
        public async Task<ActionResult> AddToCart(Guid tenantId, Guid customerId, [FromBody] JsonElement data)
        {
            try
            {
                Console.WriteLine("=== ADD TO CART ===");
                Console.WriteLine($"TenantId: {tenantId}, CustomerId: {customerId}");

                // Xác nhận customer thuộc tenant
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Id == customerId && c.TenantId == tenantId);

                if (customer == null)
                    return NotFound(new { message = "Không tìm thấy khách hàng!" });

                // Parse JSON
                if (!data.TryGetProperty("productSkuId", out var skuIdProp) ||
                    !Guid.TryParse(skuIdProp.GetString(), out Guid productSkuId))
                    return BadRequest(new { message = "productSkuId không hợp lệ!" });

                int quantity = 1;
                if (data.TryGetProperty("quantity", out var qtyProp) && qtyProp.ValueKind == JsonValueKind.Number)
                    quantity = qtyProp.GetInt32();

                if (quantity <= 0)
                    return BadRequest(new { message = "Số lượng phải lớn hơn 0!" });

                // Xác nhận SKU thuộc tenant và còn hàng
                var sku = await _context.ProductSkus
                    .Include(s => s.Product)
                    .FirstOrDefaultAsync(s => s.Id == productSkuId && s.Product.TenantId == tenantId);

                if (sku == null)
                    return NotFound(new { message = "Không tìm thấy SKU sản phẩm!" });

                if (sku.Stock < quantity)
                    return BadRequest(new { message = $"SKU chỉ còn {sku.Stock} trong kho!" });

                // Tìm hoặc tạo Cart cho customer
                var cart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.CustomerId == customerId);

                if (cart == null)
                {
                    cart = new Cart { Id = Guid.NewGuid(), CustomerId = customerId };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // Kiểm tra nếu đã có item này trong giỏ (cùng SKU + selectedOptions)
                var existing = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cart.Id
                                            && ci.ProductSkuId == productSkuId);

                if (existing != null)
                {
                    existing.Quantity += quantity;
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        id = existing.Id,
                        productSkuId = existing.ProductSkuId,
                        quantity = existing.Quantity,
                        message = "Đã cập nhật số lượng trong giỏ hàng!"
                    });
                }

                // Tạo mới cart item
                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductSkuId = productSkuId,
                    Quantity = quantity
                };

                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ CartItem created: {cartItem.Id}");

                return Ok(new
                {
                    id = cartItem.Id,
                    productSkuId = cartItem.ProductSkuId,
                    quantity = cartItem.Quantity,
                    message = "Đã thêm vào giỏ hàng!"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                return BadRequest(new
                {
                    message = "Lỗi khi thêm vào giỏ hàng",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // PUT: Cập nhật số lượng của một cart item
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCartItem(Guid tenantId, Guid customerId, Guid id, [FromBody] JsonElement data)
        {
            try
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Id == customerId && c.TenantId == tenantId);

                if (customer == null)
                    return NotFound(new { message = "Không tìm thấy khách hàng!" });

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.Id == id && ci.Cart.CustomerId == customerId);

                if (cartItem == null)
                    return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng!" });

                if (!data.TryGetProperty("quantity", out var qtyProp) || qtyProp.ValueKind != JsonValueKind.Number)
                    return BadRequest(new { message = "Thiếu hoặc sai định dạng quantity!" });

                int newQuantity = qtyProp.GetInt32();
                if (newQuantity <= 0)
                    return BadRequest(new { message = "Số lượng phải lớn hơn 0!" });

                // Kiem tra ton kho qua SKU
                var sku = await _context.ProductSkus
                    .FirstOrDefaultAsync(s => s.Id == cartItem.ProductSkuId);
                if (sku != null && sku.Stock < newQuantity)
                    return BadRequest(new { message = $"SKU chỉ còn {sku.Stock} trong kho!" });

                cartItem.Quantity = newQuantity;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = cartItem.Id,
                    productSkuId = cartItem.ProductSkuId,
                    quantity = cartItem.Quantity,
                    message = "Đã cập nhật giỏ hàng!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Lỗi khi cập nhật giỏ hàng",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // DELETE: Xóa một item khỏi giỏ hàng
        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveCartItem(Guid tenantId, Guid customerId, Guid id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId && c.TenantId == tenantId);

            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng!" });

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == id && ci.Cart.CustomerId == customerId);

            if (cartItem == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng!" });

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng!" });
        }

        // DELETE: Xóa toàn bộ giỏ hàng
        [HttpDelete]
        public async Task<ActionResult> ClearCart(Guid tenantId, Guid customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId && c.TenantId == tenantId);

            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng!" });

            var items = await _context.CartItems
                .Where(ci => ci.Cart.CustomerId == customerId)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã xóa {items.Count} sản phẩm khỏi giỏ hàng!" });
        }
    }
}
