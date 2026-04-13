using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluxifyAPI.Services.Interfaces;

namespace FluxifyAPI.Controllers
{
    // Class này sẽ chứa các API dành cho admin, như quản lý người dùng, quản lý sản phẩm, v.v.
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]

    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        [HttpGet("platformUsers")]
        public async Task<IActionResult> GetAllPlatformUsers()
        {
            var result = await _adminService.GetAllPlatformUsersAsync();
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }

        [HttpDelete("platformUsers/{id}")]
        public async Task<IActionResult> DeletePlatformUser(Guid id)
        {
            var result = await _adminService.DeletePlatformUserAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.Message });

            return StatusCode(result.StatusCode, result.Data);
        }
    }
}

