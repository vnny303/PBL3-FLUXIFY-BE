using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopifyAPI.Data;
using ShopifyAPI.Models;
using System.Text.Json;

namespace ShopifyAPI.Controllers
{
    [Route("api/tenants/{tenantId}/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────
        // PRODUCT ENDPOINTS
        // ─────────────────────────────────────────────

        // GET: Lay tat ca san pham (kem SKUs)
        [HttpGet]
        public async Task<ActionResult> GetProducts(Guid tenantId)
        {
            var products = await _context.Products
                .Where(p => p.TenantId == tenantId)
                .Include(p => p.Category)
                .Include(p => p.ProductSkus)
                .ToListAsync();

            return Ok(products);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(Guid tenantId, Guid id)
        {
            var product = await _context.Products
                .Where(p => p.TenantId == tenantId && p.Id == id)
                .Include(p => p.Category)
                .Include(p => p.ProductSkus)
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: Tao san pham moi (co the kem danh sach SKUs)
        // Body mau:
        // {
        //   "name": "Ao thun",
        //   "description": "...",
        //   "categoryId": "...",
        //   "isActive": true,
        //   "attributes": {"color":["Do","Xanh"],"size":["S","M","L"]},
        //   "skus": [
        //     { "price": 150000, "stock": 10, "attributes": {"color":"Do","size":"S"} },
        //     { "price": 160000, "stock": 5,  "attributes": {"color":"Xanh","size":"M"} }
        //   ]
        // }
        [HttpPost]
        public async Task<ActionResult> CreateProduct(Guid tenantId, [FromBody] JsonElement data)
        {
            try
            {
                string name = data.GetProperty("name").GetString() ?? "";
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest(new { message = "Ten san pham khong duoc de trong!" });

                string? description = data.TryGetProperty("description", out var descProp)
                    ? descProp.GetString() : null;

                Guid? categoryId = null;
                if (data.TryGetProperty("categoryId", out var catProp))
                {
                    string? catStr = catProp.GetString();
                    if (!string.IsNullOrEmpty(catStr) && Guid.TryParse(catStr, out Guid catGuid))
                        categoryId = catGuid;
                }

                bool isActive = !data.TryGetProperty("isActive", out var activeProp) || activeProp.GetBoolean();

                // attributes JSON: dinh nghia cac nhom tuy chon cua san pham
                // Vi du: {"color":["Do","Xanh"],"size":["S","M","L"]}
                string? attributes = null;
                if (data.TryGetProperty("attributes", out var attrProp) && attrProp.ValueKind != JsonValueKind.Null)
                    attributes = attrProp.GetRawText();

                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    CategoryId = categoryId,
                    Name = name.Trim(),
                    Description = description?.Trim(),
                    IsActive = isActive,
                    Attributes = attributes
                };

                _context.Products.Add(product);

                // Tao SKUs neu duoc gui kem
                var skuResults = new List<object>();
                if (data.TryGetProperty("skus", out var skusProp) && skusProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var skuEl in skusProp.EnumerateArray())
                    {
                        var sku = ParseSkuElement(skuEl, product.Id);
                        _context.ProductSkus.Add(sku);
                        skuResults.Add(MapSku(sku));
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = product.Id,
                    name = product.Name,
                    description = product.Description,
                    categoryId = product.CategoryId,
                    isActive = product.IsActive,
                    attributes = product.Attributes,
                    skus = skuResults
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Loi khi tao san pham", error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        // PUT: Cap nhat thong tin san pham (khong bao gom SKUs)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid tenantId, Guid id, [FromBody] JsonElement data)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == id);

                if (product == null)
                    return NotFound(new { message = "Khong tim thay san pham!" });

                if (data.TryGetProperty("name", out var nameProp))
                    product.Name = nameProp.GetString() ?? product.Name;

                if (data.TryGetProperty("description", out var descProp))
                    product.Description = descProp.GetString();

                if (data.TryGetProperty("categoryId", out var catProp))
                {
                    string? catStr = catProp.GetString();
                    product.CategoryId = string.IsNullOrEmpty(catStr) ? null
                        : Guid.TryParse(catStr, out Guid g) ? g : product.CategoryId;
                }

                if (data.TryGetProperty("isActive", out var activeProp))
                    product.IsActive = activeProp.GetBoolean();

                if (data.TryGetProperty("attributes", out var attrProp))
                    product.Attributes = attrProp.ValueKind == JsonValueKind.Null
                        ? null : attrProp.GetRawText();

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = product.Id,
                    name = product.Name,
                    description = product.Description,
                    categoryId = product.CategoryId,
                    isActive = product.IsActive,
                    attributes = product.Attributes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Loi khi cap nhat", error = ex.Message });
            }
        }

        // DELETE: Xoa san pham (cascade xoa ca SKUs)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid tenantId, Guid id)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == id);

                if (product == null)
                    return NotFound();

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Xoa thanh cong!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Loi khi xoa", error = ex.Message });
            }
        }

        // ─────────────────────────────────────────────
        // SKU ENDPOINTS  (nested under product)
        // ─────────────────────────────────────────────

        // GET: Lay tat ca SKUs cua mot san pham
        [HttpGet("{id}/skus")]
        public async Task<ActionResult> GetSkus(Guid tenantId, Guid id)
        {
            var exists = await _context.Products.AnyAsync(p => p.TenantId == tenantId && p.Id == id);
            if (!exists)
                return NotFound(new { message = "Khong tim thay san pham!" });

            var skus = await _context.ProductSkus
                .Where(s => s.ProductId == id)
                .ToListAsync();

            return Ok(skus.Select(MapSku));
        }

        // POST: Them SKU cho san pham
        // Body mau: { "price": 150000, "stock": 10, "attributes": {"color":"Do","size":"M"} }
        [HttpPost("{id}/skus")]
        public async Task<ActionResult> CreateSku(Guid tenantId, Guid id, [FromBody] JsonElement data)
        {
            try
            {
                var exists = await _context.Products.AnyAsync(p => p.TenantId == tenantId && p.Id == id);
                if (!exists)
                    return NotFound(new { message = "Khong tim thay san pham!" });

                var sku = ParseSkuElement(data, id);
                _context.ProductSkus.Add(sku);
                await _context.SaveChangesAsync();

                return Ok(MapSku(sku));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Loi khi tao SKU", error = ex.Message });
            }
        }

        // PUT: Cap nhat SKU
        [HttpPut("{id}/skus/{skuId}")]
        public async Task<IActionResult> UpdateSku(Guid tenantId, Guid id, Guid skuId, [FromBody] JsonElement data)
        {
            try
            {
                var sku = await _context.ProductSkus
                    .Include(s => s.Product)
                    .FirstOrDefaultAsync(s => s.Id == skuId && s.ProductId == id && s.Product.TenantId == tenantId);

                if (sku == null)
                    return NotFound(new { message = "Khong tim thay SKU!" });

                if (data.TryGetProperty("price", out var priceProp))
                    sku.Price = priceProp.ValueKind == JsonValueKind.Number
                        ? priceProp.GetDecimal()
                        : decimal.TryParse(priceProp.GetString(), out decimal p) ? p : sku.Price;

                if (data.TryGetProperty("stock", out var stockProp))
                    sku.Stock = stockProp.ValueKind == JsonValueKind.Number
                        ? stockProp.GetInt32()
                        : int.TryParse(stockProp.GetString(), out int s) ? s : sku.Stock;

                if (data.TryGetProperty("attributes", out var attrProp))
                    sku.Attributes = attrProp.ValueKind == JsonValueKind.Null
                        ? null : attrProp.GetRawText();

                await _context.SaveChangesAsync();
                return Ok(MapSku(sku));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Loi khi cap nhat SKU", error = ex.Message });
            }
        }

        // DELETE: Xoa SKU
        [HttpDelete("{id}/skus/{skuId}")]
        public async Task<IActionResult> DeleteSku(Guid tenantId, Guid id, Guid skuId)
        {
            try
            {
                var sku = await _context.ProductSkus
                    .Include(s => s.Product)
                    .FirstOrDefaultAsync(s => s.Id == skuId && s.ProductId == id && s.Product.TenantId == tenantId);

                if (sku == null)
                    return NotFound(new { message = "Khong tim thay SKU!" });

                _context.ProductSkus.Remove(sku);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Xoa SKU thanh cong!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Loi khi xoa SKU", error = ex.Message });
            }
        }

        // ─────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────

        private static ProductSku ParseSkuElement(JsonElement el, Guid productId)
        {
            decimal price = 0;
            if (el.TryGetProperty("price", out var priceProp))
                price = priceProp.ValueKind == JsonValueKind.Number
                    ? priceProp.GetDecimal()
                    : decimal.TryParse(priceProp.GetString(), out decimal p) ? p : 0;

            int stock = 0;
            if (el.TryGetProperty("stock", out var stockProp))
                stock = stockProp.ValueKind == JsonValueKind.Number
                    ? stockProp.GetInt32()
                    : int.TryParse(stockProp.GetString(), out int s) ? s : 0;

            string? attributes = null;
            if (el.TryGetProperty("attributes", out var attrProp) && attrProp.ValueKind != JsonValueKind.Null)
                attributes = attrProp.GetRawText();

            return new ProductSku
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Price = price,
                Stock = stock,
                Attributes = attributes
            };
        }

        private static object MapSku(ProductSku sku) => new
        {
            id = sku.Id,
            productId = sku.ProductId,
            price = sku.Price,
            stock = sku.Stock,
            attributes = sku.Attributes
        };
    }
}