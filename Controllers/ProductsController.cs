using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluxifyAPI.DTOs.Product;
using FluxifyAPI.DTOs.ProductSku;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Interfaces;

namespace FluxifyAPI.Controllers
{
    [Authorize(Roles = "merchant")]
    [Route("api/tenants/{tenantId}/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // ---------------------------------------------
        // PRODUCT ENDPOINTS
        // ---------------------------------------------

        // GET: Lay tat ca san pham (kem SKUs)
        [HttpGet]
        public async Task<ActionResult> GetProducts(Guid tenantId, [FromQuery] QueryProduct query)
        {
            var result = await _productService.GetProductsAsync(tenantId, query);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(Guid tenantId, Guid id)
        {
            var result = await _productService.GetProductAsync(tenantId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.CreateProductAsync(tenantId, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // PUT: Cap nhat thong tin san pham (khong bao gom SKUs)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid tenantId, Guid id, [FromBody] UpdateProductRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.UpdateProductAsync(tenantId, id, updateDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // DELETE: Xoa san pham (cascade xoa ca SKUs)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid tenantId, Guid id)
        {
            var result = await _productService.DeleteProductAsync(tenantId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // ---------------------------------------------
        // SKU ENDPOINTS  (nested under product)
        // ---------------------------------------------

        // GET: Lay tat ca SKUs cua mot san pham
        [HttpGet("{id}/skus")]
        public async Task<ActionResult> GetSkus(Guid tenantId, Guid id)
        {
            var result = await _productService.GetSkusAsync(tenantId, id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // POST: Them SKU cho san pham
        // Body mau: { "price": 150000, "stock": 10, "attributes": {"color":"Do","size":"M"} }
        [HttpPost("{id}/skus")]
        public async Task<ActionResult> CreateSku(Guid tenantId, Guid id, [FromBody] CreateProductSkuRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.CreateSkuAsync(tenantId, id, createDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // PUT: Cap nhat SKU
        [HttpPut("{id}/skus/{skuId}")]
        public async Task<IActionResult> UpdateSku(Guid tenantId, Guid id, Guid skuId, [FromBody] UpdateProductSkuRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.UpdateSkuAsync(tenantId, id, skuId, updateDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        // DELETE: Xoa SKU
        [HttpDelete("{id}/skus/{skuId}")]
        public async Task<IActionResult> DeleteSku(Guid tenantId, Guid id, Guid skuId)
        {
            var result = await _productService.DeleteSkuAsync(tenantId, id, skuId);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}

