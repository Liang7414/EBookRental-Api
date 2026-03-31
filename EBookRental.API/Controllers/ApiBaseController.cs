using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EBookRental.API.Controllers
{
    // 會需要用到 JWT Token 來驗證的 API Controller，才會繼承這個 ApiBaseController，其餘的使用ControllerBase 就好
    [Route("api/[controller]")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected int GetUserToken()
        {
            // 從 JWT Token 中取得 userId
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 如果 userIdClaim 是 null 或空字串，或者無法轉換成 int，則拋出未授權異常
            if (string.IsNullOrEmpty(userIdClaim)||!int.TryParse(userIdClaim, out int userId)){ 
                throw new UnauthorizedAccessException("無效的通行證");
            }

            return userId;
        }
    }
}
