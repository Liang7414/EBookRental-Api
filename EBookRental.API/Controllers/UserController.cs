using Microsoft.AspNetCore.Mvc;
using EBookRental.Core.DTOs;
using EBookRental.Core.Interfaces;

namespace EBookRental.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            // 呼叫 Service 執行 BCrypt 加密與存檔
            var success = await _userService.RegisterAsync(request);

            if (!success)
            {
                return BadRequest(new { message = "帳號已存在或其他註冊錯誤" });
            }

            return Ok(new { message = "註冊成功！" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var result = await _userService.LoginAsync(request);

            if (result == null)
            {
                // 出於安全考慮，通常不會明說是密碼錯還是帳號錯，統一給 Unauthorized
                return Unauthorized(new { message = "帳號或密碼錯誤" });
            }

            return Ok(new { token = result });
        }
    }
}
