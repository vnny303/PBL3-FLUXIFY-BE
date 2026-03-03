using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopifyAPI.Data;
using ShopifyAPI.Models;
using System.Text.Json;

namespace ShopifyAPI.Controllers
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
            // Xác nhận customer thuộc tenant
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId && c.TenantId == tenantId);

            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng!" });

            var items = await _context.CartItems
                .Where(ci => ci.CustomerId == customerId)
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.ProductSkus)
                .ToListAsync();

            var result = items.Select(ci =>
            {
                // Lay gia thap nhat trong cac SKU
                var minPrice = ci.Product.ProductSkus
                    .OrderBy(s => s.Price)
                    .Select(s => (decimal?)s.Price)
                    .FirstOrDefault() ?? 0m;
                return new
                {
                    id = ci.Id,
                    productId = ci.ProductId,
                    productName = ci.Product.Name,
                    productPrice = minPrice,
                    quantity = ci.Quantity,
                    selectedOptions = ci.SelectedOptions,
                    subTotal = minPrice * ci.Quantity
                };
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
                if (!data.TryGetProperty("productId", out var productIdProp) ||
                    !Guid.TryParse(productIdProp.GetString(), out Guid productId))
                    return BadRequest(new { message = "productId không hợp lệ!" });

                int quantity = 1;
                if (data.TryGetProperty("quantity", out var qtyProp) && qtyProp.ValueKind == JsonValueKind.Number)
                    quantity = qtyProp.GetInt32();

                if (quantity <= 0)
                    return BadRequest(new { message = "Số lượng phải lớn hơn 0!" });

                string? selectedOptions = data.TryGetProperty("selectedOptions", out var optProp)
                    ? optProp.GetString()
                    : null;

                // Xac nhan san pham thuoc tenant va con hang
                var product = await _context.Products
                    .Include(p => p.ProductSkus)
                    .FirstOrDefaultAsync(p => p.Id == productId && p.TenantId == tenantId && p.IsActive == true);

                if (product == null)
                    return NotFound(new { message = "Khong tim thay san pham!" });

                // Tong stock cua tat ca SKUs
                int totalStock = product.ProductSkus.Sum(s => s.Stock);
                if (totalStock < quantity)
                    return BadRequest(new { message = $"San pham chi con {totalStock} trong kho!" });

                // Kiểm tra nếu đã có item này trong giỏ (cùng product + selectedOptions)
                var existing = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CustomerId == customerId
                                            && ci.ProductId == productId
                                            && ci.SelectedOptions == selectedOptions);

                if (existing != null)
                {
                    existing.Quantity += quantity;
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        id = existing.Id,
                        productId = existing.ProductId,
                        quantity = existing.Quantity,
                        selectedOptions = existing.SelectedOptions,
                        message = "Đã cập nhật số lượng trong giỏ hàng!"
                    });
                }

                // Tạo mới cart item
                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ProductId = productId,
                    Quantity = quantity,
                    SelectedOptions = selectedOptions
                };

                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ CartItem created: {cartItem.Id}");

                return Ok(new
                {
                    id = cartItem.Id,
                    productId = cartItem.ProductId,
                    quantity = cartItem.Quantity,
                    selectedOptions = cartItem.SelectedOptions,
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
                    .FirstOrDefaultAsync(ci => ci.Id == id && ci.CustomerId == customerId);

                if (cartItem == null)
                    return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng!" });

                if (!data.TryGetProperty("quantity", out var qtyProp) || qtyProp.ValueKind != JsonValueKind.Number)
                    return BadRequest(new { message = "Thiếu hoặc sai định dạng quantity!" });

                int newQuantity = qtyProp.GetInt32();
                if (newQuantity <= 0)
                    return BadRequest(new { message = "Số lượng phải lớn hơn 0!" });

                // Kiem tra ton kho qua SKUs
                var product = await _context.Products
                    .Include(p => p.ProductSkus)
                    .FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);
                if (product != null)
                {
                    int totalStock = product.ProductSkus.Sum(s => s.Stock);
                    if (totalStock < newQuantity)
                        return BadRequest(new { message = $"San pham chi con {totalStock} trong kho!" });
                }

                cartItem.Quantity = newQuantity;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = cartItem.Id,
                    productId = cartItem.ProductId,
                    quantity = cartItem.Quantity,
                    selectedOptions = cartItem.SelectedOptions,
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
                .FirstOrDefaultAsync(ci => ci.Id == id && ci.CustomerId == customerId);

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
                .Where(ci => ci.CustomerId == customerId)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã xóa {items.Count} sản phẩm khỏi giỏ hàng!" });
        }
    }
}
