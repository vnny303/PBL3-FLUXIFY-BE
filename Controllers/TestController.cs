// Sử dụng Tokens JWT rồi nên file này không cần thiết

// using Microsoft.AspNetCore.Mvc;

// namespace FluxifyAPI.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class TestController : ControllerBase
//     {
//         [HttpGet("session")]
//         public IActionResult GetSession()
//         {
//             return Ok(new
//             {
//                 userId = HttpContext.Session.GetString("UserId"),
//                 email = HttpContext.Session.GetString("Email"),
//                 role = HttpContext.Session.GetString("Role"),
//                 tenantId = HttpContext.Session.GetString("TenantId")
//             });
//         }
//     }
// }