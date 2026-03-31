using EBookRental.Core.DTOs;
using EBookRental.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EBookRental.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ApiBaseController
    {
        private readonly IRentalService _rentalService;
        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpPost("RentBook")]
        public async Task<IActionResult> RentBook([FromBody] RentalRequestDto request)
        {
            int userIdFromToken = GetUserToken();

            try
            {
                // 呼叫 Service 執行租書邏輯（包含加一個月）
                var result = await _rentalService.RentBookAsync(userIdFromToken,request);

                // 回傳 200 OK 與處理好的 DTO
                return Ok(result);
            }
            catch (Exception ex)
            {
                // 如果 Service 丟出「找不到書籍」等錯誤，這裡會捕捉並回傳 400
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetRecords")] // 使用 GET 請求
        public async Task<IActionResult> GetAll()
        {
            // 同樣透過 BaseController 的方法安全取得 ID
            var userId = GetUserToken();

            var rentals = await _rentalService.GetRentalsByUserAsync(userId);

            return Ok(rentals);
        }
    }
}

    
