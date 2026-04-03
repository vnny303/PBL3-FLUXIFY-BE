using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluxifyAPI.Data;
using FluxifyAPI.DTOs.Product;
using FluxifyAPI.DTOs.ProductSku;
using FluxifyAPI.Models;
using FluxifyAPI.Mapper;

namespace FluxifyAPI.Controllers
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

            return Ok(products.Select(p => p.ToProductDto()));
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

            return Ok(product.ToProductDto());
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
        public async Task<ActionResult> CreateProduct(Guid tenantId, [FromBody] CreateProductRequestDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == createDto.CategoryId && c.TenantId == tenantId);
                if (!categoryExists)
                    return BadRequest(new { message = "Category không tồn tại trong tenant này" });

                var product = createDto.ToProductFromCreateDto(tenantId);

                _context.Products.Add(product);

                await _context.SaveChangesAsync();

                return Ok(product.ToProductDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Loi khi tao san pham", error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        // PUT: Cap nhat thong tin san pham (khong bao gom SKUs)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid tenantId, Guid id, [FromBody] UpdateProductRequestDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == id);

                if (product == null)
                    return NotFound(new { message = "Khong tim thay san pham!" });

                if (updateDto.CategoryId.HasValue)
                {
                    var categoryExists = await _context.Categories.AnyAsync(c => c.Id == updateDto.CategoryId.Value && c.TenantId == tenantId);
                    if (!categoryExists)
                        return BadRequest(new { message = "Category không tồn tại trong tenant này" });
                }

                updateDto.ToProductFromUpdateDto(product);

                await _context.SaveChangesAsync();

                await _context.Entry(product).Collection(p => p.ProductSkus).LoadAsync();
                return Ok(product.ToProductDto());
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

            return Ok(skus.Select(s => s.ToProductSkuDto()));
        }

        // POST: Them SKU cho san pham
        // Body mau: { "price": 150000, "stock": 10, "attributes": {"color":"Do","size":"M"} }
        [HttpPost("{id}/skus")]
        public async Task<ActionResult> CreateSku(Guid tenantId, Guid id, [FromBody] CreateProductSkuRequestDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var exists = await _context.Products.AnyAsync(p => p.TenantId == tenantId && p.Id == id);
                if (!exists)
                    return NotFound(new { message = "Khong tim thay san pham!" });

                var sku = createDto.ToProductSkuFromCreateDto(id);
                _context.ProductSkus.Add(sku);
                await _context.SaveChangesAsync();

                return Ok(sku.ToProductSkuDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Loi khi tao SKU", error = ex.Message });
            }
        }

        // PUT: Cap nhat SKU
        [HttpPut("{id}/skus/{skuId}")]
        public async Task<IActionResult> UpdateSku(Guid tenantId, Guid id, Guid skuId, [FromBody] UpdateProductSkuRequestDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var sku = await _context.ProductSkus
                    .Include(s => s.Product)
                    .FirstOrDefaultAsync(s => s.Id == skuId && s.ProductId == id && s.Product.TenantId == tenantId);

                if (sku == null)
                    return NotFound(new { message = "Khong tim thay SKU!" });

                updateDto.ToProductSkuFromUpdateDto(sku);

                await _context.SaveChangesAsync();
                return Ok(sku.ToProductSkuDto());
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
    }
}